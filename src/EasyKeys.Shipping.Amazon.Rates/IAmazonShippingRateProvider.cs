using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Amazon.Rates;

public interface IAmazonShippingRateProvider
{
    Task<Shipment> GetRatesAsync(Shipment shipment, CancellationToken cancellationToken = default);
}
