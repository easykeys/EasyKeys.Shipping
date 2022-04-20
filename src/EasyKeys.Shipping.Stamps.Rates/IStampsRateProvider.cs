using EasyKeys.Shipping.Abstractions.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Rates
{
    public interface IStampsRateProvider
    {
        Task<List<RateV40>> GetRatesAsync(Shipment shipment, CancellationToken cancellationToken = default);
    }
}
