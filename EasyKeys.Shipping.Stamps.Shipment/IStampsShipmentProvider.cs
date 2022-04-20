
using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Shipment
{
    public interface IStampsShipmentProvider
    {
        Task<CreateIndiciumResponse> CreateShipmentAsync(Shipping.Abstractions.Models.Shipment shipment, RateV40 rate, CancellationToken cancellationToken);
    }
}
