using Ardalis.SmartEnum;

using Humanizer;

namespace EasyKeys.Shipping.Stamps.Abstractions.Models;

public abstract class StampsContentType : SmartEnum<StampsContentType>
{
    public static readonly StampsContentType Other = new OtherType();

    public static readonly StampsContentType ReturnedGoods = new ReturnedGoodsType();

    public static readonly StampsContentType Merchandise = new MerchandiseType();

    public static readonly StampsContentType HumanitarianDonation = new HumanitarianDonationType();

    public static readonly StampsContentType Gift = new GiftType();

    public static readonly StampsContentType Document = new DocumentType();

    public static readonly StampsContentType DangerousGoods = new DangerousGoodsType();

    public static readonly StampsContentType CommcercialSample = new CommercialSampleType();

    protected StampsContentType(string name, int value) : base(name, value)
    {
        Description = name.Humanize(LetterCasing.Title);
    }

    public string Description { get; }

    private sealed class OtherType : StampsContentType
    {
        public OtherType()
            : base(
                  StampsClient.v111.ContentTypeV2.Other.ToString(),
                  (int)StampsClient.v111.ContentTypeV2.Other)
        {
        }
    }

    private sealed class ReturnedGoodsType : StampsContentType
    {
        public ReturnedGoodsType()
            : base(
                StampsClient.v111.ContentTypeV2.ReturnedGoods.ToString(),
                (int)StampsClient.v111.ContentTypeV2.ReturnedGoods)
        {
        }
    }

    private sealed class MerchandiseType : StampsContentType
    {
        public MerchandiseType()
            : base(
                StampsClient.v111.ContentTypeV2.Merchandise.ToString(),
                (int)StampsClient.v111.ContentTypeV2.Merchandise)
        {
        }
    }

    private sealed class HumanitarianDonationType : StampsContentType
    {
        public HumanitarianDonationType()
            : base(
                StampsClient.v111.ContentTypeV2.HumanitarianDonation.ToString(),
                (int)StampsClient.v111.ContentTypeV2.HumanitarianDonation)
        {
        }
    }

    private sealed class GiftType : StampsContentType
    {
        public GiftType()
            : base(
                  StampsClient.v111.ContentTypeV2.Gift.ToString(),
                  (int)StampsClient.v111.ContentTypeV2.Gift)
        {
        }
    }

    private sealed class DocumentType : StampsContentType
    {
        public DocumentType()
            : base(
                  StampsClient.v111.ContentTypeV2.Document.ToString(),
                  (int)StampsClient.v111.ContentTypeV2.Document)
        {
        }
    }

    private sealed class DangerousGoodsType : StampsContentType
    {
        public DangerousGoodsType()
            : base(
                  StampsClient.v111.ContentTypeV2.DangerousGoods.ToString(),
                  (int)StampsClient.v111.ContentTypeV2.DangerousGoods)
        {
        }
    }

    private sealed class CommercialSampleType : StampsContentType
    {
        public CommercialSampleType()
            : base(
                  StampsClient.v111.ContentTypeV2.CommercialSample.ToString(),
                  (int)StampsClient.v111.ContentTypeV2.CommercialSample)
        {
        }
    }
}
