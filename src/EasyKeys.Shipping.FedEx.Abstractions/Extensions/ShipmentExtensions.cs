using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;

namespace EasyKeys.Shipping.FedEx.Abstractions.Extensions;

public static class ShipmentExtensions
{
    public static bool IsEligibleForFedExOneRate(this Shipment shipment)
    {
        return shipment.DestinationAddress.IsUnitedStatesAddress()
            && shipment.OriginAddress.IsUnitedStatesAddress()
            && shipment.Options.PackagingType != FedExPackageType.YourPackaging.Name;
    }
}
