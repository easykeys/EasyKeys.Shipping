using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.Stamps.Shipment.Models
{
    public abstract class PostageMode : SmartEnum<PostageMode>
    {
        public static readonly PostageMode NORMAL = new Normal();

        public static readonly PostageMode NO_POSTAGE = new NoPostage();

        public PostageMode(string name, int value) : base(name, value)
        {
        }

        public abstract string PostageModeName { get; }

        private sealed class Normal : PostageMode
        {
            public Normal() : base("NORMAL", 0)
            {
            }

            public override string PostageModeName => "Normal";
        }

        private sealed class NoPostage : PostageMode
        {
            public NoPostage() : base("NO_POSTAGE", 1)
            {
            }

            public override string PostageModeName => "No Postage";
        }
    }
}
