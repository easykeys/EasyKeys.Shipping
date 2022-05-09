namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ContentTypes
{
    public sealed class ReturnedGoods : ContentTypes
    {
        public ReturnedGoods() : base("Returned Goods", 3, StampsClient.v111.ContentTypeV2.ReturnedGoods)
        {
        }
    }
}
