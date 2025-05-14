using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.DHL.Rates;

public interface IDHLExpressRateProvider
{
    Task<Shipment> GetRatesAsync(Shipment shipment, CancellationToken cancellationToken = default);
}
