using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Extensions;
using EasyKeys.Shipping.Stamps.Rates.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Rates.Extensions;

public static class RateV40Extensions
{
    public static RateV40 MapToInternationalRate(this RateV40 rate, Shipment shipment, RateInternationalOptions rateOptions)
    {
        rate = rate.MapToDomesticRate(shipment, rateOptions);

        rate.DeclaredValue = rateOptions.DeclaredValue;
        rate.Observations = rateOptions.Observations;
        rate.Regulations = rateOptions.Regulations;
        rate.GEMNotes = rateOptions.GEMNotes;
        rate.Restrictions = rateOptions.Restrictions;
        rate.Prohibitions = rateOptions.Prohibitions;
        rate.MaxDimensions = rateOptions.MaxDimensions;

        return rate;
    }

    public static RateV40 MapToDomesticRate(this RateV40 rate, Shipment shipment, RateOptions rateOptions)
    {
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

        return rate;
    }
}
