using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.Stamps.Shipment.Models
{
    public abstract class ImageType : SmartEnum<ImageType>
    {
        public static readonly ImageTypes Pdf = new Pdf();

        public static readonly ImageTypes Png = new Png();

        public ImageType(string name, int value, ImageType type) : base(name, value)
        {
            Type = type;
        }

        public ImageType Type
        {
            get;
            private set;
        }
    }
}
