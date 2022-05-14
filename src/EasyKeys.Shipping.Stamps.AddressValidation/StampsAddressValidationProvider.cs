using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Services;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.AddressValidation;

public class StampsAddressValidationProvider : IStampsAddressValidationProvider
{
    private readonly IStampsClientService _stampsClient;

    public StampsAddressValidationProvider(IStampsClientService stampsClientService)
    {
        _stampsClient = stampsClientService;
    }

    public async Task<ValidateAddress> ValidateAddressAsync(ValidateAddress validateAddress, CancellationToken cancellationToken)
    {
        var request = new CleanseAddressRequest()
        {
            Address = new StampsClient.v111.Address()
            {
                FullName = "This is required for address validation",
                Address1 = validateAddress.OriginalAddress.StreetLine,
                Address2 = validateAddress.OriginalAddress.StreetLine2,
                City = validateAddress.OriginalAddress.City,
                State = validateAddress.OriginalAddress.IsUnitedStatesAddress() ? validateAddress.OriginalAddress.StateOrProvince : null,
                Province = validateAddress.OriginalAddress.IsUnitedStatesAddress() ? null : validateAddress.OriginalAddress.StateOrProvince,
                ZIPCode = validateAddress.OriginalAddress.IsUnitedStatesAddress() ? validateAddress.OriginalAddress.PostalCode : null,
                PostalCode = validateAddress.OriginalAddress.IsUnitedStatesAddress() ? null : validateAddress.OriginalAddress.PostalCode,
                Country = validateAddress.OriginalAddress.CountryCode,
            }
        };

        try
        {
            var client = _stampsClient.CreateClient();

            request.Item = await _stampsClient.GetTokenAsync(cancellationToken);

            var response = await client.CleanseAddressAsync(request);

            return VerifyAddress(response, validateAddress);
        }
        catch (Exception ex)
        {
            validateAddress.InternalErrors.Add(ex.Message);
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

            // by pass the address validation if the address is not valid
            if (request?.ProposedAddress != null)
            {
                request.ProposedAddress.IsResidential = request.OriginalAddress.IsResidential;
            }

            request?.ValidationBag.Add("ValidationResult", response?.AddressCleansingResult ?? "No result");
        }

        return request!;
    }
}
