using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Rates;

using ShipClient.v25;

namespace EasyKeys.Shipping.FedEx.ShipmentProcessor
{
    public interface IFedExShipmentProvider
    {
        Task<ProcessShipmentReply> ProcessShipmentAsync(
            Shipment shipment,
            ServiceType serviceType = ServiceType.DEFAULT,
            CancellationToken cancellationToken = default);
    }
}
