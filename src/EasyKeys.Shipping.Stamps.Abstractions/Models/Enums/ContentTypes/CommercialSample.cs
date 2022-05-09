namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ContentTypes
{
    public sealed class CommercialSample : ContentTypes
    {
        public CommercialSample() : base("Commercial Sample", 5, StampsClient.v111.ContentTypeV2.CommercialSample)
        {
        }
    }
}
