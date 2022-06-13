using EasyKeys.Shipping.Stamps.Abstractions.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Rates.Extensions;

public static class ContentTypeExtensions
{
    public static ContentTypeV2 Map(this StampsContentType contentType)
    {
        return contentType.Value switch
        {
            (int)ContentTypeV2.CommercialSample => ContentTypeV2.CommercialSample,
            (int)ContentTypeV2.DangerousGoods => ContentTypeV2.DangerousGoods,
            (int)ContentTypeV2.Document => ContentTypeV2.Document,
            (int)ContentTypeV2.Gift => ContentTypeV2.Gift,
            (int)ContentTypeV2.HumanitarianDonation => ContentTypeV2.HumanitarianDonation,
            (int)ContentTypeV2.Merchandise => ContentTypeV2.Merchandise,
            (int)ContentTypeV2.ReturnedGoods => ContentTypeV2.ReturnedGoods,
            _ => ContentTypeV2.Other
        };
    }
}
