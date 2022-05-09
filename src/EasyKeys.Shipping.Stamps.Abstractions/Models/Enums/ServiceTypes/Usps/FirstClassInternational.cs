namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ServiceTypes.Usps
{
    public sealed class FirstClassInternational : ServiceTypes
    {
        public FirstClassInternational() : base("First Class International", 9, StampsClient.v111.ServiceType.USFCI)
        {
        }
    }
}
