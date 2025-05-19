using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.DHL.Rates;

public interface IDHLExpressRateProvider
{
    /// <summary>
    /// Gets live rates for a shipment.
    /// </summary>
    /// <param name="shipment"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Shipment> GetRatesAsync(Shipment shipment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get live rates for a shipment that includes all additional services.
    /// </summary>
    /// <param name="shipment"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Shipment> GetRatesManyAsync(Shipment shipment, CancellationToken cancellationToken = default);
}
