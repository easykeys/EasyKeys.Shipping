using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.Rates
{
    public interface IStampsRateProvider
    {
        Task<Shipment> GetRatesAsync(
                                        Shipment shipment,
                                        ShipmentDetails details,
                                        Abstractions.Models.ServiceType serviceType = Abstractions.Models.ServiceType.UNKNOWN,
                                        CancellationToken cancellationToken = default);
    }
}
