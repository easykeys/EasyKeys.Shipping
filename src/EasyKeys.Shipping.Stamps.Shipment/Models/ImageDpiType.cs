using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.Stamps.Shipment.Models
{
    public abstract class ImageDpiType : SmartEnum<ImageDpiType>
    {
        public static readonly ImageDpiType DPI200 = new Dpi200();

        public static readonly ImageDpiType DPI300 = new Dpi300();

        public static readonly ImageDpiType DPI96 = new Dpi96();

        public static readonly ImageDpiType DEFAULT = new Default();

        public ImageDpiType(string name, int value) : base(name, value)
        {
        }

        public abstract string DpiName { get; }

        private sealed class Dpi200 : ImageDpiType
        {
            public Dpi200() : base("DPI200", 0)
            {
            }

            public override string DpiName => "Dpi 200";
        }

        private sealed class Dpi300 : ImageDpiType
        {
            public Dpi300() : base("DPI300", 1)
            {
            }

            public override string DpiName => "Dpi 300";
        }

        private sealed class Dpi96 : ImageDpiType
        {
            public Dpi96() : base("DPI96", 2)
            {
            }

            public override string DpiName => "Dpi 96";
        }

        private sealed class Default : ImageDpiType
        {
            public Default() : base("DEFAULT", 3)
            {
            }

            public override string DpiName => "Default";
        }
    }
}
