namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ServiceTypes.Usps
{
    public sealed class Media : ServiceTypes
    {
        public Media() : base("Media Mail", 4, StampsClient.v111.ServiceType.USMM)
        {
        }
    }
}
