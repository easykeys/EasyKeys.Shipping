namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.CarrierTypes
{
    public sealed class FedEx : CarrierTypes
    {
        public FedEx() : base("FedEx", 2, StampsClient.v111.Carrier.FedEx)
        {
        }
    }
}
