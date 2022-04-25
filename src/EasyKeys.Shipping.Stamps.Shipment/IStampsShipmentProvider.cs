
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Shipment.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Shipment
{
    public interface IStampsShipmentProvider
    {
        Task<ShipmentLabel> CreateShipmentAsync(Shipping.Abstractions.Models.Shipment shipment, ShipmentRequestDetails shipmentDetails, CancellationToken cancellationToken);

        Task<CancelIndiciumResponse> CancelShipmentAsync(ShipmentLabel shipmentLabel, CancellationToken cancellationToken);
    }
}
