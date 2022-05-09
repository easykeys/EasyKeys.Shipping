namespace EasyKeys.Shipping.Stamps.Shipment.Models.Enums.PostageModes
{
    public sealed class NoPostage : PostageModes
    {
        public NoPostage() : base("No Postage", 1, StampsClient.v111.PostageMode.NoPostage)
        {
        }
    }
}
