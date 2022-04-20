using EasyKeys.Shipping.Stamps.Abstractions.Services;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Shipment
{
    public class StampsShipmentProvider : IStampsShipmentProvider
    {
        private readonly IStampsClientService _stampsClient;

        public StampsShipmentProvider(IStampsClientService stampsClientService)
        {
            _stampsClient = stampsClientService;
        }

        public async Task<CreateIndiciumResponse> CreateShipmentAsync(Shipping.Abstractions.Models.Shipment shipment, RateV40 rate, CancellationToken cancellationToken)
        {
            var client = _stampsClient.CreateClient();

            var request = new CreateIndiciumRequest()
            {
                Item = await _stampsClient.GetTokenAsync(cancellationToken),

                IntegratorTxID = Guid.NewGuid().ToString(),

                Rate = rate,

                SampleOnly = true
            };

            try
            {
                return await client.CreateIndiciumAsync(request);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
