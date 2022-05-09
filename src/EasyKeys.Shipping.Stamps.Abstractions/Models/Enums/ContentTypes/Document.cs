namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ContentTypes
{
    public sealed class Document : ContentTypes
    {
        public Document() : base("Document", 2, StampsClient.v111.ContentTypeV2.Document)
        {
        }
    }
}
