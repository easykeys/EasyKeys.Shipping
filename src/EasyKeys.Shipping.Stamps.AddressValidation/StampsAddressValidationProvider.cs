using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Services;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.AddressValidation
{
    public class StampsAddressValidationProvider : IStampsAddressValidationProvider
    {
        private readonly IStampsClientService _stampsClient;

        public StampsAddressValidationProvider(IStampsClientService stampsClientService)
        {
            _stampsClient = stampsClientService;
        }

        public async Task<ValidateAddress> ValidateAddressAsync(ValidateAddress validateAddress, CancellationToken cancellationToken)
        {
            var client = _stampsClient.CreateClient();
            var request = new CleanseAddressRequest()
            {
                Item = await _stampsClient.GetTokenAsync(cancellationToken),
                Address = new StampsClient.v111.Address()
                {
                    FullName = "This is required for address validation",
                    Address1 = validateAddress.OriginalAddress.StreetLine,
                    Address2 = validateAddress.OriginalAddress.StreetLine2,
                    City = validateAddress.OriginalAddress.City,
                    State = validateAddress.OriginalAddress.CountryCode == "US" ? validateAddress.OriginalAddress.StateOrProvince : null,
                    Province = validateAddress.OriginalAddress.CountryCode != "US" ? validateAddress.OriginalAddress.StateOrProvince : null,
                    ZIPCode = validateAddress.OriginalAddress.CountryCode == "US" ? validateAddress.OriginalAddress.PostalCode : null,
                    PostalCode = validateAddress.OriginalAddress.CountryCode != "US" ? validateAddress.OriginalAddress.PostalCode : null,
                    Country = validateAddress.OriginalAddress.CountryCode
                }
            };
            try
            {
                return VerifyAddress(await client.CleanseAddressAsync(request), validateAddress);
            }
            catch (Exception ex)
            {
                validateAddress.InternalErrors.Add(ex.Message);
                return validateAddress;
            }
        }

        private ValidateAddress VerifyAddress(CleanseAddressResponse response, ValidateAddress validateAddress)
        {
            if (!response.AddressMatch)
            {
                if (response.CityStateZipOK)
                {
                    validateAddress.Warnings.Add(new Error { Description = response.AddressCleansingResult });
                }
                else
                {
                    validateAddress.Errors.Add(new Error { Description = response.AddressCleansingResult });
                }
            }

            validateAddress.ProposedAddress = response.CandidateAddresses.FirstOrDefault() switch
            {
                null => validateAddress.OriginalAddress,
                _ => new Shipping.Abstractions.Models.Address()
                {
                    StreetLine = response.CandidateAddresses?.FirstOrDefault()?.Address1 ?? string.Empty,
                    StreetLine2 = response.CandidateAddresses?.FirstOrDefault()?.Address2 ?? string.Empty,
                    City = response.CandidateAddresses?.FirstOrDefault()?.City ?? string.Empty,
                    StateOrProvince = response.CandidateAddresses?.FirstOrDefault()?.State ?? string.Empty,
                    CountryCode = response.CandidateAddresses?.FirstOrDefault()?.Country ?? string.Empty,
                    PostalCode = response.CandidateAddresses?.FirstOrDefault()?.ZIPCode ?? string.Empty
                }
            };

            return validateAddress;
        }
    }
}
