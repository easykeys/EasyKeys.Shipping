using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.DHL.Shipment.Models;

namespace EasyKeys.Shipping.DHL.Shipment;

public interface IDHLExpressShipmentProvider
{
    Task<ShipmentLabel> CreateShipmentAsync(Shipping.Abstractions.Models.Shipment shipment, ShippingDetails shippingDetails, CancellationToken cancellationToken = default);
}
