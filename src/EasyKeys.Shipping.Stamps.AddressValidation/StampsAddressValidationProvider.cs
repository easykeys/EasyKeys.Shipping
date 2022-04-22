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
                    Address2 = "Front Office",
                    City = validateAddress.OriginalAddress.City,
                    State = validateAddress.OriginalAddress.StateOrProvince,
                    PostalCode = validateAddress.OriginalAddress.PostalCode,
                },
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
            if (!response.AddressMatch && !response.CityStateZipOK)
            {
                validateAddress.Errors.Add(new Error { Description = response.AddressCleansingResult });
                validateAddress.ProposedAddress = new Shipping.Abstractions.Models.Address()
                {
                    StreetLine = response.CandidateAddresses[0].Address1 ?? String.Empty,
                    StreetLine2 = response.CandidateAddresses[0].Address2 ?? String.Empty,
                    City = response.CandidateAddresses[0].City ?? String.Empty,
                    StateOrProvince = response.CandidateAddresses[0].State ?? String.Empty,
                    CountryCode = response.CandidateAddresses[0].Country ?? String.Empty,
                    PostalCode = response.CandidateAddresses[0].ZIPCode ?? String.Empty
                };
                return validateAddress;
            }

            return validateAddress;
        }
    }
}
