using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.Stamps.Abstractions.Models
{
    public abstract class ContentType : SmartEnum<ContentType>
    {
        public static readonly ContentType OTHER = new Other();

        public static readonly ContentType RETURNED_GOODS = new ReturnedGoods();

        public static readonly ContentType MERCHANDISE = new Merchandise();

        public static readonly ContentType HUMANITARIAN = new Humanitarian();

        public static readonly ContentType GIFT = new Gift();

        public static readonly ContentType DOCUMENT = new Document();

        public static readonly ContentType DANGEROUS_GOODS = new DangerousGoods();

        public static readonly ContentType COMMERCIAL_SAMPLE = new CommercialSample();

        public ContentType(string name, int value) : base(name, value)
        {
        }

        public abstract string ContentName { get; }

        private sealed class Other : ContentType
        {
            public Other() : base("OTHER", 0)
            {
            }

            public override string ContentName => "Other";
        }

        private sealed class ReturnedGoods : ContentType
        {
            public ReturnedGoods() : base("RETURNED_GOODS", 1)
            {
            }

            public override string ContentName => "Returned Goods";
        }

        private sealed class Merchandise : ContentType
        {
            public Merchandise() : base("MERCHANDISE", 2)
            {
            }

            public override string ContentName => "Merchandise";
        }

        private sealed class Humanitarian : ContentType
        {
            public Humanitarian() : base("HUMANITARIAN", 3)
            {
            }

            public override string ContentName => "Humanitarian";
        }

        private sealed class Gift : ContentType
        {
            public Gift() : base("GIFT", 4)
            {
            }

            public override string ContentName => "Gift";
        }

        private sealed class Document : ContentType
        {
            public Document() : base("DOCUMENT", 5)
            {
            }

            public override string ContentName => "Document";
        }

        private sealed class DangerousGoods : ContentType
        {
            public DangerousGoods() : base("DANGEROUS_GOODS", 6)
            {
            }

            public override string ContentName => "Dangerous Goods";
        }

        private sealed class CommercialSample : ContentType
        {
            public CommercialSample() : base("COMMERCIAL_SAMPLE", 7)
            {
            }

            public override string ContentName => "Commercial Sample";
        }
    }
}
