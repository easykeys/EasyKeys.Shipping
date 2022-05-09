namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ServiceTypes.Usps
{
    public sealed class PriorityExpressInternational : ServiceTypes
    {
        public PriorityExpressInternational() : base("Priority Mail Express International", 7, StampsClient.v111.ServiceType.USEMI)
        {
        }
    }
}
