using EasyKeys.Shipping.Abstractions.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Abstractions.Services
{
    public interface IGetRatesV40
    {
        Task<List<RateV40>> GetRatesResponseAsync(Shipment shipment, ShipmentDetails details, Abstractions.Models.ServiceType serviceType, CancellationToken cancellationToken = default);
    }
}
