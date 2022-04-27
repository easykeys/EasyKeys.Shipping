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
                    State = validateAddress.OriginalAddress.StateOrProvince,
                    Province = validateAddress.OriginalAddress.StateOrProvince,
                    PostalCode = validateAddress.OriginalAddress.PostalCode,
                    Country = validateAddress.OriginalAddress.CountryCode
                }
            };
            try
            {
                return VerifyAddress(await client.CleanseAddressAsync(request), validateAddress);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
                    StreetLine = response.CandidateAddresses?.FirstOrDefault()?.Address1 ?? String.Empty,
                    StreetLine2 = response.CandidateAddresses?.FirstOrDefault()?.Address2 ?? String.Empty,
                    City = response.CandidateAddresses?.FirstOrDefault()?.City ?? String.Empty,
                    StateOrProvince = response.CandidateAddresses?.FirstOrDefault()?.State ?? String.Empty,
                    CountryCode = response.CandidateAddresses?.FirstOrDefault()?.Country ?? String.Empty,
                    PostalCode = response.CandidateAddresses?.FirstOrDefault()?.ZIPCode ?? String.Empty
                }
            };

            return validateAddress;
        }
    }
}
