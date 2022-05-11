using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.Stamps.Abstractions.Models
{
    public abstract class CarrierType : SmartEnum<CarrierType>
    {
        public static readonly CarrierType FEDEX = new FedEx();

        public static readonly CarrierType UPS = new Ups();

        public static readonly CarrierType USPS = new Usps();

        public static readonly CarrierType DHL_EXPRESS = new DhlExpress();

        public CarrierType(string name, int value) : base(name, value)
        {
        }

        public abstract string CarrierName { get; }

        private sealed class FedEx : CarrierType
        {
            public FedEx() : base("FEDEX", 0)
            {
            }

            public override string CarrierName => "FedEx";
        }

        private sealed class Ups : CarrierType
        {
            public Ups() : base("UPS", 1)
            {
            }

            public override string CarrierName => "Ups";
        }

        private sealed class Usps : CarrierType
        {
            public Usps() : base("USPS", 2)
            {
            }

            public override string CarrierName => "Usps";
        }

        private sealed class DhlExpress : CarrierType
        {
            public DhlExpress() : base("DHL_EXPRESS", 3)
            {
            }

            public override string CarrierName => "Dhl Express";
        }
    }
}
