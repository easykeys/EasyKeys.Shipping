namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.CarrierTypes
{
    public sealed class Ups : CarrierTypes
    {
        public Ups() : base("Ups", 4, StampsClient.v111.Carrier.UPS)
        {
        }
    }
}
