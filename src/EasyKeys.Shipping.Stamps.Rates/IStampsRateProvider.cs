using EasyKeys.Shipping.Abstractions.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Rates
{
    public interface IStampsRateProvider
    {
        Task<GetRatesResponse> GetRatesAsync(Shipment shipment, CancellationToken cancellationToken = default);
    }
}
