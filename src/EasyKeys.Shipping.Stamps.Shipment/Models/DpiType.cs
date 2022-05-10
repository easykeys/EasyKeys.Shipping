using Ardalis.SmartEnum;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Shipment.Models
{
    public abstract class DpiType : SmartEnum<DpiType>
    {
        public static readonly DpiType Default = new Default();

        public DpiType(string name, int value, EltronPrinterDPIType type) : base(name, value)
        {
            Type = type;
        }

        public EltronPrinterDPIType Type
        {
            get;
            private set;
        }
    }
}
