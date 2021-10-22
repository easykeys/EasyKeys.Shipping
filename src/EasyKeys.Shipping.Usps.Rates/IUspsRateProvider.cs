using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Usps.Rates;

public interface IUspsRateProvider
{
    Task<Shipment> GetRatesAsync(Shipment shipment, UspsRateOptions rateOptions, CancellationToken cancellationToken = default);
}
