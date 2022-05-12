using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Abstractions.Extensions;

public static class ShipmentExtensions
{
    /// <summary>
    /// returns true if both origin and destination are USA based.
    /// </summary>
    /// <param name="shipment"></param>
    /// <returns></returns>
    public static bool IsDomestic(this Shipment shipment)
    {
        return shipment.OriginAddress.IsUnitedStatesAddress() && shipment.DestinationAddress.IsUnitedStatesAddress();
    }

    /// <summary>
    /// Returns total weight for the <see cref="Shipment"/>.
    /// </summary>
    /// <param name="shipment"></param>
    /// <returns></returns>
    public static decimal GetTotalWeight(this Shipment shipment)
    {
        return shipment.Packages.Sum(p => p.Weight);
    }
}
