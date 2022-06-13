using Ardalis.SmartEnum;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Abstractions.Models;

public abstract class StampsServiceType : SmartEnum<StampsServiceType>
{
    public static readonly StampsServiceType FirstClass = new FirstClassType();
    public static readonly StampsServiceType FirstClassInternational = new FirstClassInternationalType();
    public static readonly StampsServiceType Priority = new PriorityType();
    public static readonly StampsServiceType PriorityExpress = new PriorityExpressType();
    public static readonly StampsServiceType PriorityExpressInternational = new PriorityExpressInternationalType();
    public static readonly StampsServiceType PriorityInternational = new PriorityInternationalType();
    public static readonly StampsServiceType ParcelSelectGround = new ParcelSelectGroundType();
    public static readonly StampsServiceType MediaMail = new MediaMailType();
    public static readonly StampsServiceType PayOnUseReturn = new PayOnUseReturnType();
    public static readonly StampsServiceType LibraryMail = new LibraryMailType();
    public static readonly StampsServiceType Unknown = new UnknownType();

    protected StampsServiceType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }

    public string Description { get; private set; }

    private sealed class FirstClassType : StampsServiceType
    {
        public FirstClassType() : base(nameof(ServiceType.USFC), (int)ServiceType.USFC, "USPS First Class Mail")
        {
        }
    }

    private sealed class FirstClassInternationalType : StampsServiceType
    {
        public FirstClassInternationalType() : base(nameof(ServiceType.USFCI), (int)ServiceType.USFCI, "USPS First Class Mail International")
        {
        }
    }

    private sealed class PriorityType : StampsServiceType
    {
        public PriorityType() : base(nameof(ServiceType.USPM), (int)ServiceType.USPM, "USPS Priority Mail")
        {
        }
    }

    private sealed class PriorityExpressType : StampsServiceType
    {
        public PriorityExpressType() : base(nameof(ServiceType.USXM), (int)ServiceType.USXM, "USPS Priority Mail Express")
        {
        }
    }

    private sealed class PriorityExpressInternationalType : StampsServiceType
    {
        public PriorityExpressInternationalType() : base(nameof(ServiceType.USEMI), (int)ServiceType.USEMI, "USPS Priority Mail Express International")
        {
        }
    }

    private sealed class PriorityInternationalType : StampsServiceType
    {
        public PriorityInternationalType() : base(nameof(ServiceType.USPMI), (int)ServiceType.USPMI, "USPS Priority Mail International")
        {
        }
    }

    private sealed class ParcelSelectGroundType : StampsServiceType
    {
        public ParcelSelectGroundType() : base(nameof(ServiceType.USPS), (int)ServiceType.USPS, "USPS Parcel Select Ground")
        {
        }
    }

    private sealed class MediaMailType : StampsServiceType
    {
        public MediaMailType() : base(nameof(ServiceType.USMM), (int)ServiceType.USMM, "USPS Media Mail")
        {
        }
    }

    private sealed class PayOnUseReturnType : StampsServiceType
    {
        public PayOnUseReturnType() : base(nameof(ServiceType.USRETURN), (int)ServiceType.USRETURN, "Pay On Use Return")
        {
        }
    }

    private sealed class LibraryMailType : StampsServiceType
    {
        public LibraryMailType() : base(nameof(ServiceType.USLM), (int)ServiceType.USLM, "USPS Library Mail")
        {
        }
    }

    private sealed class UnknownType : StampsServiceType
    {
        public UnknownType() : base(nameof(ServiceType.Unknown), (int)ServiceType.Unknown, "Unknown")
        {
        }
    }
}
