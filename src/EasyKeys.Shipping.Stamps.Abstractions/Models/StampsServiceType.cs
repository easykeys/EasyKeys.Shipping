using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.Stamps.Abstractions.Models
{
    public abstract class StampsServiceType : SmartEnum<StampsServiceType>
    {
        public static readonly StampsServiceType USPS_FIRST_CLASS_MAIL = new USPS_First_Class_Mail();
        public static readonly StampsServiceType USPS_FIRST_CLASS_MAIL_INTERNATIONAL = new USPS_First_Class_Mail_International();
        public static readonly StampsServiceType USPS_PRIORITY_MAIL = new USPS_Priority_Mail();
        public static readonly StampsServiceType USPS_PRIORITY_MAIL_EXPRESS = new USPS_Priority_Mail_Express();
        public static readonly StampsServiceType USPS_PRIORITY_MAIL_EXPRESS_INTERNATIONAL = new USPS_Priority_Mail_Express_International();
        public static readonly StampsServiceType USPS_PRIORITY_MAIL_INTERNATIONAL = new USPS_Priority_Mail_International();
        public static readonly StampsServiceType USPS_PARCEL_SELECT_GROUND = new USPS_Parcel_Select_Ground();
        public static readonly StampsServiceType USPS_MEDIA_MAIL = new USPS_Media_Mail();
        public static readonly StampsServiceType USPS_PAY_ON_USE_RETURN = new USPS_Pay_On_Use_Return();
        public static readonly StampsServiceType USPS_LIBRARY_MAIL = new USPS_Library_Mail();
        public static readonly StampsServiceType UNKOWN = new Unkown();

        public StampsServiceType(string name, int value) : base(name, value)
        {
        }

        public abstract string ServiceName { get; }

        private sealed class USPS_First_Class_Mail : StampsServiceType
        {
            public USPS_First_Class_Mail() : base("USFC", 0)
            {
            }

            public override string ServiceName => "USPS First Class Mail";
        }

        private sealed class USPS_First_Class_Mail_International : StampsServiceType
        {
            public USPS_First_Class_Mail_International() : base("USFCI", 1)
            {
            }

            public override string ServiceName => "USPS First Class Mail International";
        }

        private sealed class USPS_Priority_Mail : StampsServiceType
        {
            public USPS_Priority_Mail() : base("USPM", 1)
            {
            }

            public override string ServiceName => "USPS Priority Mail";
        }

        private sealed class USPS_Priority_Mail_Express : StampsServiceType
        {
            public USPS_Priority_Mail_Express() : base("USXM", 1)
            {
            }

            public override string ServiceName => "USPS Priority Mail Express";
        }

        private sealed class USPS_Priority_Mail_Express_International : StampsServiceType
        {
            public USPS_Priority_Mail_Express_International() : base("USEMI", 1)
            {
            }

            public override string ServiceName => "USPS Priority Mail Express International";
        }

        private sealed class USPS_Priority_Mail_International : StampsServiceType
        {
            public USPS_Priority_Mail_International() : base("USPMI", 1)
            {
            }

            public override string ServiceName => "USPS Priority Mail International";
        }

        private sealed class USPS_Parcel_Select_Ground : StampsServiceType
        {
            public USPS_Parcel_Select_Ground() : base("USPS", 1)
            {
            }

            public override string ServiceName => "USPS Parcel Select Ground";
        }

        private sealed class USPS_Media_Mail : StampsServiceType
        {
            public USPS_Media_Mail() : base("USMM", 1)
            {
            }

            public override string ServiceName => "USPS Media Mail";
        }


        private sealed class USPS_Pay_On_Use_Return : StampsServiceType
        {
            public USPS_Pay_On_Use_Return() : base("USRETURN", 1)
            {
            }

            public override string ServiceName => "USPS Pay On Use Return";
        }

        private sealed class USPS_Library_Mail : StampsServiceType
        {
            public USPS_Library_Mail() : base("USLM", 1)
            {
            }

            public override string ServiceName => "USPS Library Mail";
        }

        private sealed class Unkown : StampsServiceType
        {
            public Unkown() : base("Unkown", 1)
            {
            }

            public override string ServiceName => "Unknown";
        }
    }
}
