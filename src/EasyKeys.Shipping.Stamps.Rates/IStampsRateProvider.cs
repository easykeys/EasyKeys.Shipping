using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.Rates;

public interface IStampsRateProvider
{
    /// <summary>
    /// Gets the rates data for the <see cref="Shipment"/>.
    /// </summary>
    /// <param name="shipment">shipment.</param>
    /// <param name="rateDetails"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Shipment> GetRatesAsync(Shipment shipment, RateRequestDetails rateDetails, CancellationToken cancellationToken = default);
}
