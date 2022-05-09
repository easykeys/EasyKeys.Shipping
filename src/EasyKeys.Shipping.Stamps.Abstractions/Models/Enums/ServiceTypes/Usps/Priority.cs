namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ServiceTypes.Usps
{
    public sealed class Priority : ServiceTypes
    {
        public Priority() : base("Priority Mail", 2, StampsClient.v111.ServiceType.USPM)
        {
        }
    }
}
