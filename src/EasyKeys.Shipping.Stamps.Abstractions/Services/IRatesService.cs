using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Abstractions.Services
{
    public interface IRatesService
    {
        Task<List<RateV40>> GetRatesResponseAsync(Shipment shipment, RateRequestDetails rateDetails, CancellationToken cancellationToken);
    }
}
