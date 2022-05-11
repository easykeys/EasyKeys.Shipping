using Ardalis.SmartEnum;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Abstractions.Models
{
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

        public StampsServiceType(string name, int value, string description) : base(name, value)
        {
            Description = description;
        }

        public string Description { get; private set; }

        private sealed class FirstClassType : StampsServiceType
        {
            public FirstClassType() : base(ServiceType.USFC.ToString(), (int)ServiceType.USFC, "USPS First Class Mail")
            {
            }
        }

        private sealed class FirstClassInternationalType : StampsServiceType
        {
            public FirstClassInternationalType() : base(ServiceType.USFCI.ToString(), (int)ServiceType.USFCI, "USPS First Class Mail International")
            {
            }
        }

        private sealed class PriorityType : StampsServiceType
        {
            public PriorityType() : base(ServiceType.USPM.ToString(), (int)ServiceType.USPM, "USPS Priority Mail")
            {
            }
        }

        private sealed class PriorityExpressType : StampsServiceType
        {
            public PriorityExpressType() : base(ServiceType.USXM.ToString(), (int)ServiceType.USXM, "USPS Priority Mail Express")
            {
            }
        }

        private sealed class PriorityExpressInternationalType : StampsServiceType
        {
            public PriorityExpressInternationalType() : base(ServiceType.USEMI.ToString(), (int)ServiceType.USEMI, "USPS Priority Mail Express International")
            {
            }
        }

        private sealed class PriorityInternationalType : StampsServiceType
        {
            public PriorityInternationalType() : base(ServiceType.USPMI.ToString(), (int)ServiceType.USPMI, "USPS Priority Mail International")
            {
            }
        }

        private sealed class ParcelSelectGroundType : StampsServiceType
        {
            public ParcelSelectGroundType() : base(ServiceType.USPS.ToString(), (int)ServiceType.USPS, "USPS Parcel Select Ground")
            {
            }
        }

        private sealed class MediaMailType : StampsServiceType
        {
            public MediaMailType() : base(ServiceType.USMM.ToString(), (int)ServiceType.USMM, "USPS Media Mail")
            {
            }
        }

        private sealed class PayOnUseReturnType : StampsServiceType
        {
            public PayOnUseReturnType() : base(ServiceType.USRETURN.ToString(), (int)ServiceType.USRETURN, "Pay On Use Return")
            {
            }
        }

        private sealed class LibraryMailType : StampsServiceType
        {
            public LibraryMailType() : base(ServiceType.USLM.ToString(), (int)ServiceType.USLM, "USPS Library Mail")
            {
            }
        }

        private sealed class UnknownType : StampsServiceType
        {
            public UnknownType() : base(ServiceType.Unknown.ToString(), (int)ServiceType.Unknown, "Unknown")
            {
            }
        }
    }
}
