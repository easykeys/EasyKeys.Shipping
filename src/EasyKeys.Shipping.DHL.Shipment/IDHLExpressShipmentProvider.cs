using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.DHL.Shipment;

public interface IDHLExpressShipmentProvider
{
    Task<ShipmentLabel> CreateShipmentAsync(Shipping.Abstractions.Models.Shipment shipment, CancellationToken cancellationToken = default);
}
