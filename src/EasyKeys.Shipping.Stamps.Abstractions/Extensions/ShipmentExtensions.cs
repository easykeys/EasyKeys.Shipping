using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Abstractions.Extensions;

public static class ShipmentExtensions
{
    public static RateV40 MapToRate(this Shipment shipment)
    {
        var rate = new RateV40
        {
            // take date passed and then add at least a day to it
            ShipDate = shipment.Options.ShippingDate,

            InsuredValue = shipment.Packages.Sum(x => x.InsuredValue),

            DeclaredValue = shipment.Packages.Sum(x => x.InsuredValue),

            PackageType = StampsPackageType.FromName(shipment.Options.PackagingType).Value switch
            {
                (int)PackageTypeV11.Pak => PackageTypeV11.Pak,
                (int)PackageTypeV11.Package => PackageTypeV11.Package,
                (int)PackageTypeV11.OversizedPackage => PackageTypeV11.OversizedPackage,
                (int)PackageTypeV11.LargePackage => PackageTypeV11.LargePackage,
                (int)PackageTypeV11.Postcard => PackageTypeV11.Postcard,
                (int)PackageTypeV11.Documents => PackageTypeV11.Documents,
                (int)PackageTypeV11.ThickEnvelope => PackageTypeV11.ThickEnvelope,
                (int)PackageTypeV11.Envelope => PackageTypeV11.Envelope,
                (int)PackageTypeV11.ExpressEnvelope => PackageTypeV11.ExpressEnvelope,
                (int)PackageTypeV11.FlatRateEnvelope => PackageTypeV11.FlatRateEnvelope,
                (int)PackageTypeV11.LegalFlatRateEnvelope => PackageTypeV11.LegalFlatRateEnvelope,
                (int)PackageTypeV11.Letter => PackageTypeV11.Letter,
                (int)PackageTypeV11.LargeEnvelopeorFlat => PackageTypeV11.LargeEnvelopeorFlat,
                (int)PackageTypeV11.SmallFlatRateBox => PackageTypeV11.SmallFlatRateBox,
                (int)PackageTypeV11.FlatRateBox => PackageTypeV11.FlatRateBox,
                (int)PackageTypeV11.LargeFlatRateBox => PackageTypeV11.LargeFlatRateBox,
                (int)PackageTypeV11.FlatRatePaddedEnvelope => PackageTypeV11.FlatRatePaddedEnvelope,
                (int)PackageTypeV11.RegionalRateBoxA => PackageTypeV11.RegionalRateBoxA,
                (int)PackageTypeV11.RegionalRateBoxB => PackageTypeV11.RegionalRateBoxB,
                (int)PackageTypeV11.RegionalRateBoxC => PackageTypeV11.RegionalRateBoxC,
                _ => PackageTypeV11.Package
            },

            WeightLb = shipment.Packages.Sum(x => x.PoundsAndOunces.Pounds),

            WeightOz = 0.0,

            Length = (double)(shipment.Packages.FirstOrDefault()?.Dimensions.Length ?? 0.0m),

            Width = (double)(shipment.Packages?.FirstOrDefault()?.Dimensions.Width ?? 0.0m),

            Height = (double)(shipment.Packages?.FirstOrDefault()?.Dimensions.Height ?? 0.0m)
        };

        // priority express & priority express international service types do not except any box package types
        if (rate.ServiceType == ServiceType.USXM || rate.ServiceType == ServiceType.USEMI)
        {
            rate.PackageType = rate.PackageType.ToString().Contains("Box") ? PackageTypeV11.Package : rate.PackageType;
        }

        return rate;
    }
}
