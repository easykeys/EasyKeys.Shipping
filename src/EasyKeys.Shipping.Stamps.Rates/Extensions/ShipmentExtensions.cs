using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Extensions;
using EasyKeys.Shipping.Stamps.Rates.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Rates.Extensions;

public static class ShipmentExtensions
{
    public static RateV40 MapToDomesticRate(this Shipment shipment, RateOptions rateOptions)
    {
        return shipment.MapToRate().MapToDomesticRate(shipment, rateOptions);
    }

    public static RateV40 MapToInternationalRate(this Shipment shipment, RateInternationalOptions rateOptions)
    {
        return shipment.MapToRate().MapToInternationalRate(shipment, rateOptions);
    }
}
