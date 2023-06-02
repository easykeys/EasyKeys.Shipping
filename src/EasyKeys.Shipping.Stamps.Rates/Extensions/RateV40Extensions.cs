using EasyKeys.Shipping.Abstractions.Extensions;
using EasyKeys.Shipping.Stamps.Abstractions.Extensions;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Rates.Extensions;

public static class RateV40Extensions
{
    public static RateV40 MapToRate(
        this RateV40 rate,
        Shipping.Abstractions.Models.Shipment shipment,
        Models.RateOptions rateOptions)
    {
        // take date passed and then add at least a day to it
        rate.ShipDate = shipment.Options.ShippingDate;

        rate.InsuredValue = shipment.Packages.Sum(x => x.InsuredValue);

        rate.DeclaredValue = shipment.Packages.Sum(x => x.InsuredValue);

        rate.PackageType = Abstractions.Models.StampsPackageType.FromName(shipment.Options.PackagingType).Value switch
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
        };

        rate.WeightLb = shipment.Packages.Sum(x => x.PoundsAndOunces.Pounds);

        rate.WeightOz = shipment.Packages.Sum(x => x.PoundsAndOunces.Ounces);

        rate.Length = (double)(shipment.Packages.FirstOrDefault()?.Dimensions.Length ?? 0.0m);

        rate.Width = (double)(shipment.Packages?.FirstOrDefault()?.Dimensions.Width ?? 0.0m);

        rate.Height = (double)(shipment.Packages?.FirstOrDefault()?.Dimensions.Height ?? 0.0m);

        // priority express & priority express international service types do not except any box package types
        if (rate.ServiceType == ServiceType.USXM || rate.ServiceType == ServiceType.USEMI)
        {
            rate.PackageType = rate.PackageType.ToString().Contains("Box") ? PackageTypeV11.Package : rate.PackageType;
        }

        rate.From = shipment.OriginAddress.Map(rateOptions.Sender);

        rate.To = shipment.DestinationAddress.Map(rateOptions.Recipient);

        rate.Amount = rateOptions.Amount;

        rate.MaxAmount = rateOptions.MaxAmount;

        rate.ServiceType = rateOptions.ServiceType.Value switch
        {
            (int)ServiceType.USPS => ServiceType.USPS,
            (int)ServiceType.USFC => ServiceType.USFC,
            (int)ServiceType.USMM => ServiceType.USMM,
            (int)ServiceType.USPM => ServiceType.USPM,
            (int)ServiceType.USXM => ServiceType.USXM,
            (int)ServiceType.USEMI => ServiceType.USEMI,
            (int)ServiceType.USFCI => ServiceType.USFCI,
            (int)ServiceType.USRETURN => ServiceType.USRETURN,
            (int)ServiceType.USLM => ServiceType.USLM,
            (int)ServiceType.USPMI => ServiceType.USPMI,
            (int)ServiceType.Unknown => ServiceType.Unknown,
            _ => ServiceType.Unknown
        };

        rate.ServiceDescription = rateOptions.ServiceType.Description;

        rate.RegisteredValue = rateOptions.RegisteredValue;

        rate.CODValue = rateOptions.CODValue;

        rate.NonMachinable = rateOptions.NonMachinable;

        rate.RectangularShaped = rateOptions.RectangularShaped;

        rate.DimWeighting = rateOptions.DimWeighting;

        rate.CubicPricing = rateOptions.CubicPricing;

        rate.ContentType = rateOptions.ContentType.Map();

        if (!shipment.IsDomestic())
        {
            rate.DeclaredValue = rateOptions.DeclaredValue;
            rate.Observations = rateOptions.Observations;
            rate.Regulations = rateOptions.Regulations;
            rate.GEMNotes = rateOptions.GEMNotes;
            rate.Restrictions = rateOptions.Restrictions;
            rate.Prohibitions = rateOptions.Prohibitions;
            rate.MaxDimensions = rateOptions.MaxDimensions;
        }

        if (shipment?.Packages?.Sum(x => x.InsuredValue) > 0)
        {
            var addons = new AddOnV17[0];

            rate.AddOns = addons.Append(new AddOnV17()
            {
                AddOnType = AddOnTypeV17.PGAINS
            }).ToArray();
        }

        return rate;
    }
}
