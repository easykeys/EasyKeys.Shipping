using Ardalis.SmartEnum;

using EasyKeys.Shipping.Stamps.Shipment.Models.Enums.PrinterDpiTypes;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Shipment.Models.Enums.PrinterTypes
{
    public abstract class DpiTypes : SmartEnum<DpiTypes>
    {
        public static readonly DpiTypes Default = new Default();

        public DpiTypes(string name, int value, EltronPrinterDPIType type) : base(name, value)
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
