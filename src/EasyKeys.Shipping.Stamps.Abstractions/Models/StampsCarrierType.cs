using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.Stamps.Abstractions.Models;

public abstract class StampsCarrierType : SmartEnum<StampsCarrierType>
{
    public static readonly StampsCarrierType FedEx = new FedExType();

    public static readonly StampsCarrierType Ups = new UpsType();

    public static readonly StampsCarrierType Usps = new UspsType();

    public static readonly StampsCarrierType DhlExpress = new DhlExpressType();

    protected StampsCarrierType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }

    public string Description { get; }

    private sealed class FedExType : StampsCarrierType
    {
        public FedExType()
            : base(nameof(StampsClient.v111.Carrier.FedEx), (int)StampsClient.v111.Carrier.FedEx, "FedEx")
        {
        }
    }

    private sealed class UpsType : StampsCarrierType
    {
        public UpsType()
            : base(nameof(StampsClient.v111.Carrier.UPS), (int)StampsClient.v111.Carrier.UPS, "UPS")
        {
        }
    }

    private sealed class UspsType : StampsCarrierType
    {
        public UspsType()
            : base(nameof(StampsClient.v111.Carrier.USPS), (int)StampsClient.v111.Carrier.USPS, "USPS")
        {
        }
    }

    private sealed class DhlExpressType : StampsCarrierType
    {
        public DhlExpressType()
            : base(nameof(StampsClient.v111.Carrier.DHLExpress), (int)StampsClient.v111.Carrier.DHLExpress, "DHL Express")
        {
        }
    }
}
