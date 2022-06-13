using Ardalis.SmartEnum;

using ShipClient.v25;

namespace EasyKeys.Shipping.FedEx.Shipment.Models;

public abstract class FedExCollectionType : SmartEnum<FedExCollectionType>
{
    public static readonly FedExCollectionType GuaranteedFunds = new GuaranteedFundsType();
    public static readonly FedExCollectionType Cash = new CashType();
    public static readonly FedExCollectionType Any = new AnyType();
    public static readonly FedExCollectionType CompanyCheck = new CompanyCheckType();

    protected FedExCollectionType(string name, int value) : base(name, value)
    {
    }

    private class GuaranteedFundsType : FedExCollectionType
    {
        public GuaranteedFundsType() : base(CodCollectionType.GUARANTEED_FUNDS.ToString(), (int)CodCollectionType.GUARANTEED_FUNDS)
        {
        }
    }

    private class CashType : FedExCollectionType
    {
        public CashType() : base(CodCollectionType.CASH.ToString(), (int)CodCollectionType.CASH)
        {
        }
    }

    private class AnyType : FedExCollectionType
    {
        public AnyType() : base(CodCollectionType.ANY.ToString(), (int)CodCollectionType.ANY)
        {
        }
    }

    private class CompanyCheckType : FedExCollectionType
    {
        public CompanyCheckType() : base(CodCollectionType.COMPANY_CHECK.ToString(), (int)CodCollectionType.COMPANY_CHECK)
        {
        }
    }
}
