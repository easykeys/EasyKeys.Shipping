
using EasyKeys.Shipping.Abstractions.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Shipment
{
    public interface IStampsShipmentProvider
    {
        Task<ShipmentLabel> CreateShipmentAsync(Shipping.Abstractions.Models.Shipment shipment, RateV40 rate, CancellationToken cancellationToken);

        Task<CancelIndiciumResponse> CancelShipmentAsync(ShipmentLabel shipmentLabel, CancellationToken cancellationToken);
    }
}
