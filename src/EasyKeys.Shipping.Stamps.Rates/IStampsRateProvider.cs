using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.Rates;

public interface IStampsRateProvider
{
    /// <summary>
    /// Gets the rates data for the <see cref="Shipment"/>.
    /// </summary>
    /// <param name="shipments">The list of shipments.</param>
    /// <param name="rateDetails"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Shipment> GetRatesAsync(List<Shipment> shipments, RateRequestDetails rateDetails, CancellationToken cancellationToken = default);
}
