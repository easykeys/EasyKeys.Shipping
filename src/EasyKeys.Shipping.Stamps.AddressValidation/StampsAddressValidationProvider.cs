using System.Diagnostics;

using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Services;
using EasyKeys.Shipping.Stamps.AddressValidation.Extensions;

using Microsoft.Extensions.Logging;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.AddressValidation;

public class StampsAddressValidationProvider : IStampsAddressValidationProvider, IAddressValidationProvider
{
    private readonly IStampsClientService _stampsClient;
    private readonly ILogger<StampsAddressValidationProvider> _logger;

    public StampsAddressValidationProvider(
        IStampsClientService stampsClientService,
        ILogger<StampsAddressValidationProvider> logger)
    {
        _stampsClient = stampsClientService ?? throw new ArgumentNullException(nameof(stampsClientService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string Name => nameof(StampsAddressValidationProvider);

    public async Task<ValidateAddress> ValidateAddressAsync(ValidateAddress validateAddress, CancellationToken cancellationToken)
    {
        var request = new CleanseAddressRequest()
        {
            Address = validateAddress.OriginalAddress.GetStampsAddress(),
        };

        try
        {
            var watch = ValueStopwatch.StartNew();

            var response = await _stampsClient.CleanseAddressAsync(request, cancellationToken);

            _logger.LogDebug("[Stamps.com][ValidateAddressAsync] completed: {mil}", watch.GetElapsedTime().TotalMilliseconds);

            return VerifyAddress(response, validateAddress);
        }
        catch (Exception ex)
        {
            _logger.LogError("{name} : {message}", nameof(StampsAddressValidationProvider), ex.Message);

            validateAddress.InternalErrors.Add(ex.Message);

            validateAddress.ValidationBag.Remove("CityStateZipOK");
            validateAddress.ValidationBag.Add("CityStateZipOK", "false");

            validateAddress.ValidationBag.Remove("AddressMatch");
            validateAddress.ValidationBag.Add("AddressMatch", "false");

            validateAddress.ValidationBag.Remove("IsPOBox");
            validateAddress.ValidationBag.Add("IsPOBox", "false");

            validateAddress.ValidationBag.Remove("ValidationResult");
            validateAddress.ValidationBag.Add("ValidationResult", ex.Message);

            return validateAddress;
        }
    }

    private ValidateAddress VerifyAddress(CleanseAddressResponse response, ValidateAddress request)
    {
        if (request != null)
        {
            request.ValidationBag.Remove("CityStateZipOK");
            request.ValidationBag.Add("CityStateZipOK", $"{response.CityStateZipOK}");

            request.ValidationBag.Remove("AddressMatch");
            request.ValidationBag.Add("AddressMatch", $"{response.AddressMatch}");

            request.ValidationBag.Remove("IsPOBox");
            request.ValidationBag.Add("IsPOBox", $"{response.IsPOBox}");

            var cleansedAddress = response.CandidateAddresses.FirstOrDefault();

            if (cleansedAddress != null)
            {
                request.ProposedAddress = new Shipping.Abstractions.Models.Address()
                {
                    StreetLine = cleansedAddress?.Address1 ?? string.Empty,
                    StreetLine2 = cleansedAddress?.Address2 ?? string.Empty,
                    City = cleansedAddress?.City ?? string.Empty,
                    StateOrProvince = cleansedAddress?.State ?? (cleansedAddress?.Province ?? string.Empty),
                    CountryCode = cleansedAddress?.Country ?? string.Empty,
                    PostalCode = cleansedAddress?.ZIPCode ?? (cleansedAddress?.PostalCode ?? string.Empty)
                };
            }
            else
            {
                request.ProposedAddress = new Shipping.Abstractions.Models.Address()
                {
                    StreetLine = response?.Address?.Address1 ?? string.Empty,
                    StreetLine2 = response?.Address?.Address2 ?? string.Empty,
                    City = response?.Address?.City ?? string.Empty,
                    StateOrProvince = response?.Address?.State ?? (response?.Address?.Province ?? string.Empty),
                    CountryCode = response?.Address?.Country ?? string.Empty,
                    PostalCode = response?.Address?.ZIPCode ?? (response?.Address?.PostalCode ?? string.Empty)
                };
            }

            request.ProposedAddress.IsResidential = request.OriginalAddress.IsResidential;

            request.ValidationBag.Remove("ValidationResult");
            request.ValidationBag.Add("ValidationResult", response?.AddressCleansingResult ?? "No result");
        }

        return request!;
    }
}
