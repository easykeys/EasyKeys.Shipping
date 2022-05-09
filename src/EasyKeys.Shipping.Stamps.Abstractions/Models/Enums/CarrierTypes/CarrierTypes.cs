using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.CarrierTypes
{
    public abstract class CarrierTypes : SmartEnum<CarrierTypes>
    {
        public static readonly CarrierTypes Fedex = new FedEx();

        public static readonly CarrierTypes Ups = new Ups();

        public static readonly CarrierTypes Usps = new Usps();

        public static readonly DhlExpress DhsExpress = new DhlExpress();

        public CarrierTypes(string name, int value, StampsClient.v111.Carrier type) : base(name, value)
        {
            Type = type;
        }

        public StampsClient.v111.Carrier Type { get; private set; }
    }
}
