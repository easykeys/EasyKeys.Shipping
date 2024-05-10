using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Shipment.Models;

namespace EasyKeys.Shipping.FedEx.Shipment.RestApi.Impl;
public class FedExShipmentProvider : IFedExShipmentProvider
{
    public Task<ShipmentCancelledResult> CancelShipmentAsync(string trackingId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ShipmentLabel> CreateShipmentAsync(FedExServiceType serviceType, Shipping.Abstractions.Models.Shipment shipment, ShipmentDetails shipmentDetails, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
