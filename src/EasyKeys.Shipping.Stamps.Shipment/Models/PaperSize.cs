using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.Stamps.Shipment.Models
{
    public abstract class PaperSize : SmartEnum<PaperSize>
    {
        public static readonly PaperSize DEFAULT = new Default();

        public static readonly PaperSize LETTER_85X11 = new Letter85x11();

        public static readonly PaperSize LABEL_SIZE = new LabelSize();

        public PaperSize(string name, int value) : base(name, value)
        {
        }

        public abstract string PaperSizeName { get; }

        private sealed class Letter85x11 : PaperSize
        {
            public Letter85x11() : base("LETTER_85X11", 0)
            {
            }

            public override string PaperSizeName => "Letter 85x11";
        }

        private sealed class LabelSize : PaperSize
        {
            public LabelSize() : base("LABEL_SIZE", 1)
            {
            }

            public override string PaperSizeName => "Label Size";
        }

        private sealed class Default : PaperSize
        {
            public Default() : base("DEFAULT", 2)
            {
            }

            public override string PaperSizeName => "Default";
        }
    }
}
