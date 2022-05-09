using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ContentTypes
{
    public abstract class ContentTypes : SmartEnum<ContentTypes>
    {
        public static readonly ContentTypes Other = new Other();

        public static readonly ContentTypes ReturnedGoods = new ReturnedGoods();

        public static readonly ContentTypes Merchandise = new Merchandise();

        public static readonly ContentTypes Humanitarian = new Humanitarian();

        public static readonly ContentTypes Gift = new Gift();

        public static readonly ContentTypes Document = new Document();

        public static readonly ContentTypes DangerousGoods = new DangerousGoods();

        public static readonly ContentTypes CommercialSample = new CommercialSample();

        public ContentTypes(string name, int value, StampsClient.v111.ContentTypeV2 type) : base(name, value)
        {
            Type = type;
        }

        public StampsClient.v111.ContentTypeV2 Type { get; private set; }
    }
}
