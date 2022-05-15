using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.FedEx.Abstractions.Models;

public abstract class FedExServiceType : SmartEnum<FedExServiceType>
{
    // 0
    public static readonly FedExServiceType Default = new DefaultType();

    // 1
    public static readonly FedExServiceType EuropeFirstInternationalPriority = new EuropeFirstInternationalPriorityType();

    // 2
    public static readonly FedExServiceType FedExOneDayFreight = new FedExOneDayFreightType();

    // 3
    public static readonly FedExServiceType FedExSecondDay = new FedExSecondDayType();

    // 4
    public static readonly FedExServiceType FedExSecondDayAM = new FedExSecondDayAMType();

    // 5
    public static readonly FedExServiceType FedExSecondDayFreight = new FedExSecondDayFreightType();

    // 6
    public static readonly FedExServiceType FedExThirdDayFreight = new FedExThirdDayFreightType();

    // 7
    public static readonly FedExServiceType FedExExpressSaver = new FedExExpressSaverType();

    // 8
    public static readonly FedExServiceType FedExFirstFreight = new FedExFirstFreightType();

    // 9
    public static readonly FedExServiceType FedExFreightEconomy = new FedExFreightEconomyType();

    // 10
    public static readonly FedExServiceType FedExFreightPriority = new FedExFreightPriorityType();

    // 11
    public static readonly FedExServiceType FedExGround = new FedExGroundType();

    // 12
    public static readonly FedExServiceType FedExFirstOvernight = new FedExFirstOvernightType();

    // 13
    public static readonly FedExServiceType FedExGroundHomeDelivery = new FedExGroundHomeDeliveryType();

    // 14
    public static readonly FedExServiceType FedExInternationalEconomy = new FedExInternationalEconomyType();

    // 15
    public static readonly FedExServiceType FedExInternationalEconomyFreight = new FedExInternationalEconomyFreightType();

    // 16
    public static readonly FedExServiceType FedExInternationalFirst = new FedExInternationalFirstType();

    // 17
    public static readonly FedExServiceType FedExInternationalPriority = new FedExInternationalPriorityType();

    // 18
    public static readonly FedExServiceType FedExInternationalPriorityFreight = new FedExInternationalPriorityFreightType();

    // 19
    public static readonly FedExServiceType FedExPriorityOvernight = new FedExPriorityOvernightType();

    // 20
    public static readonly FedExServiceType FedExSmartPost = new FedExSmartPostType();

    // 21
    public static readonly FedExServiceType FedExStandardOvernight = new FedExStandardOvernightType();

    // 22
    public static readonly FedExServiceType FedExInternationalPriorityExpress = new FedExInternationalPriorityExpressType();

    private FedExServiceType(string name, int value) : base(name, value)
    {
    }

    public abstract string ServiceName { get; }

    private sealed class DefaultType : FedExServiceType
    {
        public DefaultType() : base("DEFAULT", 0)
        {
        }

        public override string ServiceName => "Default Service";
    }

    private sealed class EuropeFirstInternationalPriorityType : FedExServiceType
    {
        public EuropeFirstInternationalPriorityType() : base("EUROPE_FIRST_INTERNATIONAL_PRIORITY", 1)
        {
        }

        public override string ServiceName => "FedEx Europe First International Priority";
    }

    private sealed class FedExOneDayFreightType : FedExServiceType
    {
        public FedExOneDayFreightType() : base("FEDEX_1_DAY_FREIGHT", 2)
        {
        }

        public override string ServiceName => "FedEx 1Day Freight";
    }

    private sealed class FedExSecondDayType : FedExServiceType
    {
        public FedExSecondDayType() : base("FEDEX_2_DAY", 3)
        {
        }

        public override string ServiceName => "FedEx 2Day";
    }

    private sealed class FedExSecondDayAMType : FedExServiceType
    {
        public FedExSecondDayAMType() : base("FEDEX_2_DAY_AM", 4)
        {
        }

        public override string ServiceName => "FedEx 2Day AM";
    }

    private sealed class FedExSecondDayFreightType : FedExServiceType
    {
        public FedExSecondDayFreightType() : base("FEDEX_2_DAY_FREIGHT", 5)
        {
        }

        public override string ServiceName => "FedEx 2Day Freight";
    }

    private sealed class FedExThirdDayFreightType : FedExServiceType
    {
        public FedExThirdDayFreightType() : base("FEDEX_3_DAY_FREIGHT", 6)
        {
        }

        public override string ServiceName => "FedEx 3Day Freight";
    }

    private sealed class FedExExpressSaverType : FedExServiceType
    {
        public FedExExpressSaverType() : base("FEDEX_EXPRESS_SAVER", 7)
        {
        }

        public override string ServiceName => "FedEx Express Saver";
    }

    private sealed class FedExFirstFreightType : FedExServiceType
    {
        public FedExFirstFreightType() : base("FEDEX_FIRST_FREIGHT", 8)
        {
        }

        public override string ServiceName => "FedEx First Freight";
    }

    private sealed class FedExFreightEconomyType : FedExServiceType
    {
        public FedExFreightEconomyType() : base("FEDEX_FREIGHT_ECONOMY", 9)
        {
        }

        public override string ServiceName => "Fedex Freight Economy";
    }

    private sealed class FedExFreightPriorityType : FedExServiceType
    {
        public FedExFreightPriorityType() : base("FEDEX_FREIGHT_PRIORITY", 10)
        {
        }

        public override string ServiceName => "FedEx Freight Priority";
    }

    private sealed class FedExGroundType : FedExServiceType
    {
        public FedExGroundType() : base("FEDEX_GROUND", 11)
        {
        }

        public override string ServiceName => "FedEx Ground";
    }

    private sealed class FedExFirstOvernightType : FedExServiceType
    {
        public FedExFirstOvernightType() : base("FIRST_OVERNIGHT", 12)
        {
        }

        public override string ServiceName => "FedEx First Overnight";
    }

    private sealed class FedExGroundHomeDeliveryType : FedExServiceType
    {
        public FedExGroundHomeDeliveryType() : base("GROUND_HOME_DELIVERY", 13)
        {
        }

        public override string ServiceName => "FedEx Ground Home Delivery";
    }

    private sealed class FedExInternationalEconomyType : FedExServiceType
    {
        public FedExInternationalEconomyType() : base("INTERNATIONAL_ECONOMY", 14)
        {
        }

        public override string ServiceName => "FedEx International Economy";
    }

    private sealed class FedExInternationalEconomyFreightType : FedExServiceType
    {
        public FedExInternationalEconomyFreightType() : base("INTERNATIONAL_ECONOMY_FREIGHT", 15)
        {
        }

        public override string ServiceName => "FedEx International Economy Freight";
    }

    private sealed class FedExInternationalFirstType : FedExServiceType
    {
        public FedExInternationalFirstType() : base("INTERNATIONAL_FIRST", 16)
        {
        }

        public override string ServiceName => "FedEx International First";
    }

    private sealed class FedExInternationalPriorityType : FedExServiceType
    {
        public FedExInternationalPriorityType() : base("FEDEX_INTERNATIONAL_PRIORITY", 17)
        {
        }

        public override string ServiceName => "FedEx International Priority";
    }

    private sealed class FedExInternationalPriorityFreightType : FedExServiceType
    {
        public FedExInternationalPriorityFreightType() : base("INTERNATIONAL_PRIORITY_FREIGHT", 18)
        {
        }

        public override string ServiceName => "FedEx International Priority Freight";
    }

    private sealed class FedExPriorityOvernightType : FedExServiceType
    {
        public FedExPriorityOvernightType() : base("PRIORITY_OVERNIGHT", 19)
        {
        }

        public override string ServiceName => "FedEx Priority Overnight";
    }

    private sealed class FedExSmartPostType : FedExServiceType
    {
        public FedExSmartPostType() : base("SMART_POST", 20)
        {
        }

        public override string ServiceName => "FedEx Smart Post";
    }

    private sealed class FedExStandardOvernightType : FedExServiceType
    {
        public FedExStandardOvernightType() : base("STANDARD_OVERNIGHT", 21)
        {
        }

        public override string ServiceName => "FedEx Standard Overnight";
    }

    private sealed class FedExInternationalPriorityExpressType : FedExServiceType
    {
        public FedExInternationalPriorityExpressType() : base("FEDEX_INTERNATIONAL_PRIORITY_EXPRESS", 22)
        {
        }

        public override string ServiceName => "FedEx International Priority Express";
    }
}
