using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.Rates;

public interface IStampsRateProvider
{
    Task<Shipment> GetRatesAsync(List<Shipment> shipments, RateRequestDetails rateDetails, CancellationToken cancellationToken = default);
}
