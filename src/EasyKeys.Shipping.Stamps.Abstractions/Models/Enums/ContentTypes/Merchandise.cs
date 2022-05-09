namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ContentTypes
{
    public sealed class Merchandise : ContentTypes
    {
        public Merchandise() : base("Merchandise", 5, StampsClient.v111.ContentTypeV2.Merchandise)
        {
        }
    }
}
