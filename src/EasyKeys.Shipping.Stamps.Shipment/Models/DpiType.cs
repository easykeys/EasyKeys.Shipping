using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.Stamps.Shipment.Models
{
    public abstract class DpiType : SmartEnum<DpiType>
    {
        public static readonly DpiType DEFAULT = new Default();
        public static readonly DpiType HIGH = new High();

        public DpiType(string name, int value) : base(name, value)
        {
        }

        public abstract string DpiName { get; }

        private sealed class Default : DpiType
        {
            public Default() : base("DEFAULT", 0)
            {
            }

            public override string DpiName => "Default";
        }

        private sealed class High : DpiType
        {
            public High() : base("HIGH", 1)
            {
            }

            public override string DpiName => "High";
        }
    }
}
