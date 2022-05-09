namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ContentTypes
{
    public sealed class Gift : ContentTypes
    {
        public Gift() : base("Gift", 1, StampsClient.v111.ContentTypeV2.Gift)
        {
        }
    }
}
