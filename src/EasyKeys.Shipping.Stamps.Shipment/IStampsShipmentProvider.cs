using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Rates.Models;
using EasyKeys.Shipping.Stamps.Shipment.Models;

namespace EasyKeys.Shipping.Stamps.Shipment;

public interface IStampsShipmentProvider
{
    /// <summary>
    /// Creates Shipping Label for the domestic addresses.
    /// </summary>
    /// <param name="shipment"></param>
    /// <param name="rateOptions"></param>
    /// <param name="shipmentDetails"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ShipmentLabel> CreateShipmentAsync(
        Shipping.Abstractions.Models.Shipment shipment,
        RateOptions rateOptions,
        ShipmentDetails shipmentDetails,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels shipment.
    /// </summary>
    /// <param name="trackingId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ShipmentCancelledResult> CancelShipmentAsync(string trackingId, CancellationToken cancellationToken = default);
}
