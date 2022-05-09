namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ServiceTypes.Usps
{
    public sealed class PriorityInternational : ServiceTypes
    {
        public PriorityInternational() : base("Priority Mail International", 8, StampsClient.v111.ServiceType.USPMI)
        {
        }
    }
}
