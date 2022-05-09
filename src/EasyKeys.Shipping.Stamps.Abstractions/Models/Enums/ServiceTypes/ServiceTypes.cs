using Ardalis.SmartEnum;

using EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ServiceTypes.Usps;

namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ServiceTypes
{
    public abstract class ServiceTypes : SmartEnum<ServiceTypes>
    {
        public static readonly ServiceTypes USPS_Parcel_Select_Ground = new ParcelSelectGround();

        public static readonly ServiceTypes USPS_First_Class_Mail = new FirstClass();

        public static readonly ServiceTypes USPS_Media_Mail = new Media();

        public static readonly ServiceTypes USPS_Priority_Mail = new Priority();

        public static readonly ServiceTypes USPS_Priority_Mail_Express = new PriorityExpress();

        public static readonly ServiceTypes USPS_Priority_Mail_Express_International = new PriorityExpressInternational();

        public static readonly ServiceTypes USPS_First_Class_Mail_International = new FirstClassInternational();

        public static readonly ServiceTypes USPS_Pay_On_Use_Return = new PayOnUseReturn();

        public static readonly ServiceTypes USPS_Library_Mail = new Library();

        public static readonly ServiceTypes USPS_Priority_Mail_International = new PriorityInternational();

        public static readonly ServiceTypes Unknown = new Unknown();

        public ServiceTypes(string name, int value, StampsClient.v111.ServiceType type) : base(name, value)
        {
            Type = type;
        }

        public StampsClient.v111.ServiceType Type { get; private set; }
    }
}
