using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.Stamps.Abstractions.Models
{
    public abstract class CarrierType : SmartEnum<CarrierType>
    {
        public static readonly CarrierType FedEx = new FedExType();

        public static readonly CarrierType Ups = new UpsType();

        public static readonly CarrierType Usps = new UspsType();

        public static readonly CarrierType DhlExpress = new DhlExpressType();

        public CarrierType(string name, int value, string description) : base(name, value)
        {
            Description = description;
        }

        public string Description { get; }

        private sealed class FedExType : CarrierType
        {
            public FedExType() : base(StampsClient.v111.Carrier.FedEx.ToString(), (int)StampsClient.v111.Carrier.FedEx, "FedEx")
            {
            }
        }

        private sealed class UpsType : CarrierType
        {
            public UpsType() : base(StampsClient.v111.Carrier.USPS.ToString(), (int)StampsClient.v111.Carrier.USPS, "UPS")
            {
            }
        }

        private sealed class UspsType : CarrierType
        {
            public UspsType() : base(StampsClient.v111.Carrier.USPS.ToString(), (int)StampsClient.v111.Carrier.USPS, "USPS")
            {
            }
        }

        private sealed class DhlExpressType : CarrierType
        {
            public DhlExpressType() : base(StampsClient.v111.Carrier.DHLExpress.ToString(), (int)StampsClient.v111.Carrier.DHLExpress, "DHL Express")
            {
            }
        }
    }
}
