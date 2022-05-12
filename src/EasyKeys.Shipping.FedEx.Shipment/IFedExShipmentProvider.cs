using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Shipment.Models;

namespace EasyKeys.Shipping.FedEx.Shipment;

public interface IFedExShipmentProvider
{
    /// <summary>
    /// Creates a fedex shipping label.
    /// </summary>
    /// <param name="serviceType"></param>
    /// <param name="shipment"></param>
    /// <param name="shipmentDetails"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ShipmentLabel> CreateShipmentAsync(
        FedExServiceType serviceType,
        Shipping.Abstractions.Models.Shipment shipment,
        ShipmentDetails shipmentDetails,
        CancellationToken cancellationToken = default);
}
