using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Shipment.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Shipment;

public interface IStampsShipmentProvider
{
    Task<ShipmentLabel> CreateShipmentAsync(Shipping.Abstractions.Models.Shipment shipment, ShipmentDetails shipmentDetails, CancellationToken cancellationToken);

    Task<CancelIndiciumResponse> CancelShipmentAsync(string trackingId, CancellationToken cancellationToken);
}
