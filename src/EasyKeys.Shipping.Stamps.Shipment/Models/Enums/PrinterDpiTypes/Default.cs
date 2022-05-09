using EasyKeys.Shipping.Stamps.Shipment.Models.Enums.PrinterTypes;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Shipment.Models.Enums.PrinterDpiTypes
{
    public sealed class Default : DpiTypes
    {
        public Default() : base("Default", 0, EltronPrinterDPIType.Default)
        {
        }
    }
}
