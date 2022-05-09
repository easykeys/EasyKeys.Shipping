namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ServiceTypes.Usps
{
    public sealed class ParcelSelectGround : ServiceTypes
    {
        public ParcelSelectGround() : base("Parcel Select Ground", 11, StampsClient.v111.ServiceType.USPS)
        {
        }
    }
}
