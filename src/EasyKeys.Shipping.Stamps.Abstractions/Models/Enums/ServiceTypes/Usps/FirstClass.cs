namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ServiceTypes.Usps
{
    public sealed class FirstClass : ServiceTypes
    {
        public FirstClass() : base("First Class Mail", 1, StampsClient.v111.ServiceType.USFC)
        {
        }
    }
}
