using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Rates.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Abstractions.Services;

public interface IRatesService
{
    /// <summary>
    /// Gets rates for the shipment.
    /// </summary>
    /// <param name="shipment"></param>
    /// <param name="rateOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<RateV40>> GetRatesResponseAsync(Shipment shipment, RateOptions rateOptions, CancellationToken cancellationToken);

    /// <summary>
    /// Gets International rates.
    /// </summary>
    /// <param name="shipment"></param>
    /// <param name="rateOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<RateV40>> GetInternationalRatesAsync(Shipment shipment, RateInternationalOptions rateOptions, CancellationToken cancellationToken);
}
