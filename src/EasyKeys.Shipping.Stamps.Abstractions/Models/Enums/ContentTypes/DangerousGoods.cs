namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ContentTypes
{
    public sealed class DangerousGoods : ContentTypes
    {
        public DangerousGoods() : base("Dangerous Goods", 7, StampsClient.v111.ContentTypeV2.DangerousGoods)
        {
        }
    }
}
