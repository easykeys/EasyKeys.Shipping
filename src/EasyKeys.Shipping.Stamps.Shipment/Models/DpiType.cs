using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.Stamps.Shipment.Models
{
    public abstract class DpiType : SmartEnum<DpiType>
    {
        public static readonly DpiType Default = new DefaultType();
        public static readonly DpiType High = new HighType();

        public DpiType(string name, int value, string description) : base(name, value)
        {
            Description = description;
        }

        public string Description { get; }

        private sealed class DefaultType : DpiType
        {
            public DefaultType() : base(StampsClient.v111.EltronPrinterDPIType.Default.ToString(), (int)StampsClient.v111.EltronPrinterDPIType.Default, "Electron Printer Default Dpi Type")
            {
            }
        }

        private sealed class HighType : DpiType
        {
            public HighType() : base(StampsClient.v111.EltronPrinterDPIType.High.ToString(), (int)StampsClient.v111.EltronPrinterDPIType.High, "Electron Printer High Dpi Type")
            {
            }
        }
    }
}
