using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.Stamps.Abstractions.Models
{
    public abstract class ContentType : SmartEnum<ContentType>
    {
        public static readonly ContentType Other = new OtherType();

        public static readonly ContentType ReturnedGoods = new ReturnedGoodsType();

        public static readonly ContentType Merchandise = new MerchandiseType();

        public static readonly ContentType HumanitarianDonation = new HumanitarianDonationType();

        public static readonly ContentType Gift = new GiftType();

        public static readonly ContentType Document = new DocumentType();

        public static readonly ContentType DangerousGoods = new DangerousGoodsType();

        public static readonly ContentType CommcercialSample = new CommercialSampleType();

        public ContentType(string name, int value, string description) : base(name, value)
        {
            Description = description;
        }

        public string Description { get; }

        private sealed class OtherType : ContentType
        {
            public OtherType() : base(StampsClient.v111.ContentTypeV2.Other.ToString(), (int)StampsClient.v111.ContentTypeV2.Other, "Other")
            {
            }
        }

        private sealed class ReturnedGoodsType : ContentType
        {
            public ReturnedGoodsType() : base(StampsClient.v111.ContentTypeV2.ReturnedGoods.ToString(), (int)StampsClient.v111.ContentTypeV2.ReturnedGoods, "Returned Goods")
            {
            }
        }

        private sealed class MerchandiseType : ContentType
        {
            public MerchandiseType() : base(StampsClient.v111.ContentTypeV2.Merchandise.ToString(), (int)StampsClient.v111.ContentTypeV2.Merchandise, "Merchandise")
            {
            }
        }

        private sealed class HumanitarianDonationType : ContentType
        {
            public HumanitarianDonationType() : base(StampsClient.v111.ContentTypeV2.HumanitarianDonation.ToString(), (int)StampsClient.v111.ContentTypeV2.HumanitarianDonation, "Humanitarian Donation")
            {
            }
        }

        private sealed class GiftType : ContentType
        {
            public GiftType() : base(StampsClient.v111.ContentTypeV2.Gift.ToString(), (int)StampsClient.v111.ContentTypeV2.Gift, "Gift")
            {
            }
        }

        private sealed class DocumentType : ContentType
        {
            public DocumentType() : base(StampsClient.v111.ContentTypeV2.Document.ToString(), (int)StampsClient.v111.ContentTypeV2.Document, "Document")
            {
            }
        }

        private sealed class DangerousGoodsType : ContentType
        {
            public DangerousGoodsType() : base(StampsClient.v111.ContentTypeV2.DangerousGoods.ToString(), (int)StampsClient.v111.ContentTypeV2.DangerousGoods, "Dangerous Goods")
            {
            }
        }

        private sealed class CommercialSampleType : ContentType
        {
            public CommercialSampleType() : base(StampsClient.v111.ContentTypeV2.CommercialSample.ToString(), (int)StampsClient.v111.ContentTypeV2.CommercialSample, "Commercial Ssample")
            {
            }
        }
    }
}
