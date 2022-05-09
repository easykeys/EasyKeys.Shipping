namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ServiceTypes.Usps
{
    public sealed class Library : ServiceTypes
    {
        public Library() : base("Library Mail", 6, StampsClient.v111.ServiceType.USLM)
        {
        }
    }
}
