using Ardalis.SmartEnum;

using ShipClient.v25;

namespace EasyKeys.Shipping.FedEx.Abstractions.Models;

public abstract class FedExPaymentType : SmartEnum<FedExPaymentType>
{
    public static readonly FedExPaymentType Sender = new SenderType();
    public static readonly FedExPaymentType Account = new AccountType();
    public static readonly FedExPaymentType Recipient = new RecipienType();
    public static readonly FedExPaymentType ThirdParty = new ThirdPartyType();
    public static readonly FedExPaymentType Collect = new CollectType();

    protected FedExPaymentType(string name, int value) : base(name, value)
    {
    }

    private class SenderType : FedExPaymentType
    {
        public SenderType() : base(nameof(PaymentType.SENDER), (int)PaymentType.SENDER)
        {
        }
    }

    private class AccountType : FedExPaymentType
    {
        public AccountType() : base(nameof(PaymentType.ACCOUNT), (int)PaymentType.ACCOUNT)
        {
        }
    }

    private class RecipienType : FedExPaymentType
    {
        public RecipienType() : base(nameof(PaymentType.RECIPIENT), (int)PaymentType.RECIPIENT)
        {
        }
    }

    private class ThirdPartyType : FedExPaymentType
    {
        public ThirdPartyType() : base(nameof(PaymentType.THIRD_PARTY), (int)PaymentType.THIRD_PARTY)
        {
        }
    }

    private class CollectType : FedExPaymentType
    {
        public CollectType() : base(nameof(PaymentType.COLLECT), (int)PaymentType.COLLECT)
        {
        }
    }
}
