namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ServiceTypes.Usps
{
    public sealed class PriorityExpress : ServiceTypes
    {
        public PriorityExpress() : base("Priority Mail Express", 3, StampsClient.v111.ServiceType.USXM)
        {
        }
    }
}
