namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ServiceTypes
{
    public sealed class Unknown : ServiceTypes
    {
        public Unknown() : base("Unkown", 0, StampsClient.v111.ServiceType.Unknown)
        {
        }
    }
}
