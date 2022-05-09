
using Ardalis.SmartEnum;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Shipment.Models.Enums.ImageTypes
{
    public abstract class ImageTypes : SmartEnum<ImageTypes>
    {
        public static readonly ImageTypes Pdf = new Pdf();

        public static readonly ImageTypes Png = new Png();

        public ImageTypes(string name, int value, ImageType type) : base(name, value)
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
