namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.CarrierTypes
{
    public sealed class DhlExpress : CarrierTypes
    {
        public DhlExpress() : base("DHL Express", 3, StampsClient.v111.Carrier.DHLExpress)
        {
        }
    }
}
