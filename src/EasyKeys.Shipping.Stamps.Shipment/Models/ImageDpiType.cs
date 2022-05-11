using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.Stamps.Shipment.Models
{
    public abstract class ImageDpiType : SmartEnum<ImageDpiType>
    {
        public static readonly ImageDpiType Dpi200 = new Dpi200Type();

        public static readonly ImageDpiType Dpi300 = new Dpi300Type();

        public static readonly ImageDpiType Dpi96 = new Dpi96Type();

        public static readonly ImageDpiType Default = new DefaultType();

        public ImageDpiType(string name, int value, string description) : base(name, value)
        {
            Description = description;
        }

        public string Description { get; }

        private sealed class Dpi200Type : ImageDpiType
        {
            public Dpi200Type() : base(StampsClient.v111.ImageDpi.ImageDpi200.ToString(), (int)StampsClient.v111.ImageDpi.ImageDpi200, "Image Dpi 200")
            {
            }
        }

        private sealed class Dpi300Type : ImageDpiType
        {
            public Dpi300Type() : base(StampsClient.v111.ImageDpi.ImageDpi300.ToString(), (int)StampsClient.v111.ImageDpi.ImageDpi300, "Image Dpi 300")
            {
            }
        }

        private sealed class Dpi96Type : ImageDpiType
        {
            public Dpi96Type() : base(StampsClient.v111.ImageDpi.ImageDpi96.ToString(), (int)StampsClient.v111.ImageDpi.ImageDpi96, "Image Dpi 96")
            {
            }
        }

        private sealed class DefaultType : ImageDpiType
        {
            public DefaultType() : base(StampsClient.v111.ImageDpi.ImageDpiDefault.ToString(), (int)StampsClient.v111.ImageDpi.ImageDpiDefault, "Image Dpi Default")
            {
            }
        }
    }
}
