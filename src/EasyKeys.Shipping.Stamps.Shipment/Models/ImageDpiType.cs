using Ardalis.SmartEnum;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Shipment.Models
{
    public abstract class ImageDpiType : SmartEnum<ImageDpiType>
    {
        public static readonly Dpi200 Dpi200 = new Dpi200();

        public static readonly Dpi300 Dpi300 = new Dpi300();

        public static readonly Dpi96 Dpi96 = new Dpi96();

        public static readonly DpiDefault Default = new DpiDefault();

        public ImageDpiTypes(string name, int value, ImageDpi dpi) : base(name, value)
        {
            Dpi = dpi;
        }

        public ImageDpi Dpi { get; private set; }
    }
}
