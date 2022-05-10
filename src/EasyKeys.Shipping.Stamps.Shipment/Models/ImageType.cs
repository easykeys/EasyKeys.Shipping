using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.Stamps.Shipment.Models
{
    public abstract class ImageType : SmartEnum<ImageType>
    {
        public static readonly ImageType PDF = new Pdf();

        public static readonly ImageType PNG = new Png();

        public ImageType(string name, int value) : base(name, value)
        {
        }

        public abstract string ImageTypeName { get; }

        private sealed class Pdf : ImageType
        {
            public Pdf() : base("PDF", 0)
            {
            }

            public override string ImageTypeName => "PDF";
        }

        private sealed class Png : ImageType
        {
            public Png() : base("PNG", 1)
            {
            }

            public override string ImageTypeName => "PNG";
        }
    }
}
