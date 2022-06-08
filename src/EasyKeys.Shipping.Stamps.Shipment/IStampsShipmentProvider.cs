using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Rates.Models;
using EasyKeys.Shipping.Stamps.Shipment.Models;

using StampsClient.v111;

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
    Task<ShipmentLabel> CreateDomesticShipmentAsync(
        Shipping.Abstractions.Models.Shipment shipment,
        RateOptions rateOptions,
        ShipmentDetails shipmentDetails,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates Shipping Label for the international addresses.
    /// </summary>
    /// <param name="shipment"></param>
    /// <param name="rateOptions"></param>
    /// <param name="shipmentDetails"></param>
    /// <param name="commodities"></param>
    /// <param name="customsInformation"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ShipmentLabel> CreateInternationalShipmentAsync(
        Shipping.Abstractions.Models.Shipment shipment,
        RateInternationalOptions rateOptions,
        ShipmentDetails shipmentDetails,
        IList<Commodity> commodities,
        CustomsInformation customsInformation,
        CancellationToken cancellationToken);

    /// <summary>
    /// Cancels shipment.
    /// </summary>
    /// <param name="trackingId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CancelIndiciumResponse> CancelShipmentAsync(string trackingId, CancellationToken cancellationToken);
}
