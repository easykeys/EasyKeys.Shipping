using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Rates.Models;

namespace EasyKeys.Shipping.Stamps.Rates;

public interface IStampsRateProvider
{
    /// <summary>
    /// Gets the rates data for the <see cref="Shipment"/>.
    /// </summary>
    /// <param name="shipment">shipment.</param>
    /// <param name="rateOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Shipment> GetRatesAsync(Shipment shipment, RateOptions rateOptions, CancellationToken cancellationToken);
}
