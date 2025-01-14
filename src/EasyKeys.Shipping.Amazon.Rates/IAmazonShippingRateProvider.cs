using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Amazon.Rates.Models;

namespace EasyKeys.Shipping.Amazon.Rates;

public interface IAmazonShippingRateProvider
{
    Task<Shipment> GetRatesAsync(Shipment shipment, RateContactInfo rateContactInfo, CancellationToken cancellationToken = default);
}
