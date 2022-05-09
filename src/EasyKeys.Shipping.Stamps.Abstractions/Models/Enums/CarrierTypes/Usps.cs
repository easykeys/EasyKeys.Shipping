namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.CarrierTypes
{
    public sealed class Usps : CarrierTypes
    {
        public Usps() : base("Usps", 1, StampsClient.v111.Carrier.USPS)
        {
        }
    }
}
