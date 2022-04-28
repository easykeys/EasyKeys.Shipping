using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Abstractions.Services;

public interface IRatesService
{
    /// <summary>
    /// Gets rates for the shipment.
    /// </summary>
    /// <param name="shipment"></param>
    /// <param name="rateDetails"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<RateV40>> GetRatesResponseAsync(Shipment shipment, RateRequestDetails rateDetails, CancellationToken cancellationToken);
}
