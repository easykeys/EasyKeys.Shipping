using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.Stamps.Shipment.Models
{
    public abstract class PaperSizeType : SmartEnum<PaperSizeType>
    {
        public static readonly PaperSizeType Default = new DefaultType();

        public static readonly PaperSizeType Letter85x11 = new Letter85x11Type();

        public static readonly PaperSizeType LabelSize = new LabelSizeType();

        public PaperSizeType(string name, int value, string description) : base(name, value)
        {
            Description = description;
        }

        public string Description { get; }

        private sealed class Letter85x11Type : PaperSizeType
        {
            public Letter85x11Type() : base(StampsClient.v111.PaperSizeV1.Letter85x11.ToString(), (int)StampsClient.v111.PaperSizeV1.Letter85x11, "Letter Size 85 x 11")
            {
            }
        }

        private sealed class LabelSizeType : PaperSizeType
        {
            public LabelSizeType() : base(StampsClient.v111.PaperSizeV1.LabelSize.ToString(), (int)StampsClient.v111.PaperSizeV1.LabelSize, "Label Size")
            {
            }
        }

        private sealed class DefaultType : PaperSizeType
        {
            public DefaultType() : base(StampsClient.v111.PaperSizeV1.Default.ToString(), (int)StampsClient.v111.PaperSizeV1.Default, "Default Size")
            {
            }
        }
    }
}
