namespace EasyKeys.Shipping.Stamps.Shipment.Models.Enums.PostageModes
{
    public sealed class NormalPostage : PostageModes
    {
        public NormalPostage() : base("Normal", 0, StampsClient.v111.PostageMode.Normal)
        {
        }
    }
}
