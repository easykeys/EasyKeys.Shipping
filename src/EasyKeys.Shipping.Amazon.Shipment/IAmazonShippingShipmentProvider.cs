using System;
using System.Collections.Generic;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Amazon.Shipment.Models;

namespace EasyKeys.Shipping.Amazon.Shipment;
public interface IAmazonShippingShipmentProvider
{
    Task<ShipmentLabel> CreateSmartShipmentAsync(
        Shipping.Abstractions.Models.Shipment shipment,
        ShippingDetails shippingDetails,
        CancellationToken cancellationToken = default);

    Task<ShipmentLabel> CreateShipmentAsync(
        string RateId,
        Shipping.Abstractions.Models.Shipment shipment,
        ShippingDetails shippingDetails,
        CancellationToken cancellationToken = default);

    Task<ShipmentCancelledResult> CancelShipmentAsync(
        string shipmentId,
        CancellationToken cancellationToken = default);
}
