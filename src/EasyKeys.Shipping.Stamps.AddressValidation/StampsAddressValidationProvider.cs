
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Services;
using EasyKeys.Shipping.Stamps.AddressValidation.Extensions;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.AddressValidation;

public class StampsAddressValidationProvider : IStampsAddressValidationProvider
{
    private readonly IStampsClientService _stampsClient;
    private readonly IPolicyService _policy;

    public StampsAddressValidationProvider(IStampsClientService stampsClientService, IPolicyService policy)
    {
        _stampsClient = stampsClientService;
        _policy = policy;
    }

    public async Task<ValidateAddress> ValidateAddressAsync(ValidateAddress validateAddress, CancellationToken cancellationToken)
    {
        var request = new CleanseAddressRequest()
        {
            Address = validateAddress.OriginalAddress.GetStampsAddress(),
        };

        var client = _stampsClient.CreateClient();

        try
        {
            request.Item = await _stampsClient.GetTokenAsync(cancellationToken);

            var response = await _policy.GetRetryWithRefreshToken(cancellationToken)
                                    .ExecuteAsync(async () => await client.CleanseAddressAsync(request));

            _stampsClient.SetToken(response.Authenticator);

            return VerifyAddress(response, validateAddress);
        }
        catch (Exception ex)
        {
            validateAddress.InternalErrors.Add(ex.Message);
            validateAddress.ValidationBag.Add("CityStateZipOK", "false");
            validateAddress.ValidationBag.Add("AddressMatch", "false");
            validateAddress.ValidationBag.Add("ValidationResult", ex.Message);

            return validateAddress;
        }
    }

    private ValidateAddress VerifyAddress(CleanseAddressResponse response, ValidateAddress request)
    {
        if (request != null)
        {
            request.ValidationBag.Add("CityStateZipOK", $"{response.CityStateZipOK}");

            request.ValidationBag.Add("AddressMatch", $"{response.AddressMatch}");

            var cleansedAddress = response.CandidateAddresses.FirstOrDefault();

            if (cleansedAddress != null)
            {
                request.ProposedAddress = new Shipping.Abstractions.Models.Address()
                {
                    StreetLine = response.CandidateAddresses?.FirstOrDefault()?.Address1 ?? string.Empty,
                    StreetLine2 = response.CandidateAddresses?.FirstOrDefault()?.Address2 ?? string.Empty,
                    City = response.CandidateAddresses?.FirstOrDefault()?.City ?? string.Empty,
                    StateOrProvince = response.CandidateAddresses?.FirstOrDefault()?.State ?? string.Empty,
                    CountryCode = response.CandidateAddresses?.FirstOrDefault()?.Country ?? string.Empty,
                    PostalCode = response.CandidateAddresses?.FirstOrDefault()?.ZIPCode ?? string.Empty
                };
            }
            else
            {
                request.ProposedAddress = new Shipping.Abstractions.Models.Address()
                {
                    StreetLine = response?.Address?.Address1 ?? string.Empty,
                    StreetLine2 = response?.Address?.Address2 ?? string.Empty,
                    City = response?.Address?.City ?? string.Empty,
                    StateOrProvince = response?.Address?.State ?? string.Empty,
                    CountryCode = response?.Address?.Country ?? string.Empty,
                    PostalCode = response?.Address?.ZIPCode ?? string.Empty
                };
            }

            request.ProposedAddress.IsResidential = request.OriginalAddress.IsResidential;

            request?.ValidationBag.Add("ValidationResult", response?.AddressCleansingResult ?? "No result");
        }

        return request!;
    }
}
