namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ContentTypes
{
    public sealed class Humanitarian : ContentTypes
    {
        public Humanitarian() : base("Humanitarian Donation", 5, StampsClient.v111.ContentTypeV2.HumanitarianDonation)
        {
        }
    }
}
