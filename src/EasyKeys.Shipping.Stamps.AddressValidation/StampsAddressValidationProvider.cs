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

        public async Task<Shipment> ValidateAddressAsync(Shipment shipment, CancellationToken cancellationToken)
        {
            var client = _stampsClient.CreateClient();
            var request = new CleanseAddressRequest()
            {
                Item = await _stampsClient.GetTokenAsync(cancellationToken),
                Address = new StampsClient.v111.Address()
                {
                    FullName = shipment.RecipientInformation.FullName,
                    EmailAddress = shipment.RecipientInformation.Email,
                    Address1 = shipment.DestinationAddress.StreetLine,
                    Address2 = shipment.DestinationAddress.StreetLine2,
                    City = shipment.DestinationAddress.City,
                    State = shipment.DestinationAddress.StateOrProvince,
                    PostalCode = shipment.DestinationAddress.PostalCode,
                },
            };
            try
            {
                return VerifyAddress(await client.CleanseAddressAsync(request), shipment);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private Shipment VerifyAddress(CleanseAddressResponse response, Shipment shipment)
        {
            if (!response.AddressMatch)
            {
                shipment.Errors.Add(new Error { Description = response.AddressCleansingResult });
                return shipment;
            }

            return shipment;
        }
    }
}
