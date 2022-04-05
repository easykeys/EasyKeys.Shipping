using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.FedEx.Abstractions.Models;

public abstract class FedExServiceType : SmartEnum<FedExServiceType>
{
    public static readonly FedExServiceType DEFAULT = new Default();

    private FedExServiceType(string name, int value) : base(name, value)
    {
    }

    public abstract string ServiceName { get; }

    private sealed class Default : FedExServiceType
    {
        public Default() : base("DEFAULT", 0)
        {
        }

        public override string ServiceName => "Default Service";
    }
}
