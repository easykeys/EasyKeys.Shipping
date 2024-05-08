using RateClient.v28;

namespace EasyKeys.Shipping.FedEx.Rates.WebServices.Extensions;

/// <summary>
/// Extension methods for RequestedShipment.
/// </summary>
public static class RateRequestedShipmentExtensions
{
    /// <summary>
    /// Calculates the total weight for all packages in the requested shipment.
    /// </summary>
    /// <param name="requestedShipment"></param>
    /// <returns>Calculated weight if RequestedPackageLineItems in RequestedShiment, otherwise null.</returns>
    public static Weight? GetTotalWeight(this RequestedShipment requestedShipment)
    {
        if (requestedShipment != null && requestedShipment.RequestedPackageLineItems != null && requestedShipment.RequestedPackageLineItems.Length > 0)
        {
            var weight = new Weight { Units = requestedShipment.RequestedPackageLineItems[0].Weight.Units };

            foreach (var requestedPackageLineItem in requestedShipment.RequestedPackageLineItems)
            {
                weight.Value += requestedPackageLineItem.Weight.Value;
            }

            return weight;
        }

        return default;
    }
}
