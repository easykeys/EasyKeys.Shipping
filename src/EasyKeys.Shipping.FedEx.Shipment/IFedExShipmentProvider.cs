using EasyKeys.Shipping.FedEx.Rates;

using ShipClient.v25;

namespace EasyKeys.Shipping.FedEx.Shipment
{
    public interface IFedExShipmentProvider
    {
        Task<ProcessShipmentReply> ProcessShipmentAsync(
            Shipping.Abstractions.Models.Shipment shipment,
            ServiceType serviceType = ServiceType.DEFAULT,
            CancellationToken cancellationToken = default);
    }
}
