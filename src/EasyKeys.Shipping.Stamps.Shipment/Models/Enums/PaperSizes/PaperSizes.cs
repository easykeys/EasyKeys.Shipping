using Ardalis.SmartEnum;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Shipment.Models.Enums.PaperSizes
{
    public abstract class PaperSizes : SmartEnum<PaperSizes>
    {
        public static readonly PaperSizes Default = new Default();

        public static readonly PaperSizes Letter84x11 = new Letter85x11();

        public static readonly PaperSizes LabelSize = new LabelSize();

        public PaperSizes(string name, int value, PaperSizeV1 size) : base(name, value)
        {
            Size = size;
        }

        public PaperSizeV1 Size
        {
            get;
            private set;
        }
    }
}
