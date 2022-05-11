using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.FedEx.Abstractions.Models;

public abstract class FedExServiceType : SmartEnum<FedExServiceType>
{
    public static readonly FedExServiceType DEFAULT = new Default();

    public static readonly FedExServiceType EUROPE_FIRST_INTERNATIONAL_PRIORITY = new Europe_First_International_Priority();

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

    private sealed class Europe_First_International_Priority : FedExServiceType
    {
        public Europe_First_International_Priority() : base("EUROPE_FIRST_INTERNATIONAL_PRIORITY", 0)
        {
        }

        public override string ServiceName => "FedEx Europe First International Priority";
    }

    private sealed class FedEx_1_Day_Freight : FedExServiceType
    {
        public FedEx_1_Day_Freight() : base("FEDEX_1_DAY_FREIGHT", 1)
        {
        }

        public override string ServiceName => "FedEx 1Day Freight";
    }

    private sealed class FedEx_2_Day : FedExServiceType
    {
        public FedEx_2_Day() : base("FEDEX_2_DAY", 2)
        {
        }

        public override string ServiceName => "FedEx 2Day";
    }

    private sealed class FedEx_2_Day_AM : FedExServiceType
    {
        public FedEx_2_Day_AM() : base("FEDEX_2_DAY_AM", 3)
        {
        }

        public override string ServiceName => "FedEx 2Day AM";
    }

    private sealed class FEDEX_2_DAY_FREIGHT : FedExServiceType
    {
        public FEDEX_2_DAY_FREIGHT() : base("FEDEX_2_DAY_FREIGHT", 4)
        {
        }

        public override string ServiceName => "FedEx 2Day Freight";
    }

    private sealed class FEDEX_3_DAY_FREIGHT : FedExServiceType
    {
        public FEDEX_3_DAY_FREIGHT() : base("FEDEX_3_DAY_FREIGHT", 5)
        {
        }

        public override string ServiceName => "FedEx 3Day Freight";
    }

    private sealed class FEDEX_EXPRESS_SAVER : FedExServiceType
    {
        public FEDEX_EXPRESS_SAVER() : base("FEDEX_EXPRESS_SAVER", 6)
        {
        }

        public override string ServiceName => "FedEx Express Saver";
    }

    private sealed class FEDEX_FIRST_FREIGHT : FedExServiceType
    {
        public FEDEX_FIRST_FREIGHT() : base("FEDEX_FIRST_FREIGHT", 7)
        {
        }

        public override string ServiceName => "FedEx First Freight";
    }

    private sealed class FEDEX_FREIGHT_ECONOMY : FedExServiceType
    {
        public FEDEX_FREIGHT_ECONOMY() : base("FEDEX_FREIGHT_ECONOMY", 8)
        {
        }

        public override string ServiceName => "Fedex Freight Economy";
    }

    private sealed class FEDEX_FREIGHT_PRIORITY : FedExServiceType
    {
        public FEDEX_FREIGHT_PRIORITY() : base("FEDEX_FREIGHT_PRIORITY", 9)
        {
        }

        public override string ServiceName => "FedEx Freight Priority";
    }

    private sealed class FEDEX_GROUND : FedExServiceType
    {
        public FEDEX_GROUND() : base("FEDEX_GROUND", 10)
        {
        }

        public override string ServiceName => "FedEx Ground";
    }

    private sealed class FIRST_OVERNIGHT : FedExServiceType
    {
        public FIRST_OVERNIGHT() : base("FIRST_OVERNIGHT", 11)
        {
        }

        public override string ServiceName => "FedEx First Overnight";
    }

    private sealed class GROUND_HOME_DELIVERY : FedExServiceType
    {
        public GROUND_HOME_DELIVERY() : base("GROUND_HOME_DELIVERY", 12)
        {
        }

        public override string ServiceName => "FedEx Ground Home Delivery";
    }

    private sealed class INTERNATIONAL_ECONOMY : FedExServiceType
    {
        public INTERNATIONAL_ECONOMY() : base("INTERNATIONAL_ECONOMY", 13)
        {
        }

        public override string ServiceName => "FedEx International Economy";
    }

    private sealed class INTERNATIONAL_ECONOMY_FREIGHT : FedExServiceType
    {
        public INTERNATIONAL_ECONOMY_FREIGHT() : base("INTERNATIONAL_ECONOMY_FREIGHT", 14)
        {
        }

        public override string ServiceName => "FedEx International Economy Freight";
    }

    private sealed class INTERNATIONAL_FIRST : FedExServiceType
    {
        public INTERNATIONAL_FIRST() : base("INTERNATIONAL_FIRST", 15)
        {
        }

        public override string ServiceName => "FedEx International First";
    }

    private sealed class INTERNATIONAL_PRIORITY : FedExServiceType
    {
        public INTERNATIONAL_PRIORITY() : base("INTERNATIONAL_PRIORITY", 16)
        {
        }

        public override string ServiceName => "FedEx International Priority";
    }

    private sealed class INTERNATIONAL_PRIORITY_FREIGHT : FedExServiceType
    {
        public INTERNATIONAL_PRIORITY_FREIGHT() : base("INTERNATIONAL_PRIORITY_FREIGHT", 17)
        {
        }

        public override string ServiceName => "FedEx International Priority Freight";
    }

    private sealed class PRIORITY_OVERNIGHT : FedExServiceType
    {
        public PRIORITY_OVERNIGHT() : base("PRIORITY_OVERNIGHT", 18)
        {
        }

        public override string ServiceName => "Priority Overnight";
    }

    private sealed class SMART_POST : FedExServiceType
    {
        public SMART_POST() : base("SMART_POST", 19)
        {
        }

        public override string ServiceName => "Smart Post";
    }

    private sealed class STANDARD_OVERNIGHT : FedExServiceType
    {
        public STANDARD_OVERNIGHT() : base("STANDARD_OVERNIGHT", 20)
        {
        }

        public override string ServiceName => "Standard Overnight";
    }
}
