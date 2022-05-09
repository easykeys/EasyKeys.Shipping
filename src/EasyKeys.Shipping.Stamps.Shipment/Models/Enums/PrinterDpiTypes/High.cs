
using EasyKeys.Shipping.Stamps.Shipment.Models.Enums.PrinterTypes;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Shipment.Models.Enums.PrinterDpiTypes
{
    public sealed class High : DpiTypes
    {
        public High() : base("High", 1, EltronPrinterDPIType.High)
        {
        }
    }
}
