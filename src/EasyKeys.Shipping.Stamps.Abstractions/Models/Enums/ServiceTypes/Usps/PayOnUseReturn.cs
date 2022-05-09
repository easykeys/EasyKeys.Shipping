namespace EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ServiceTypes.Usps
{
    public sealed class PayOnUseReturn : ServiceTypes
    {
        public PayOnUseReturn() : base("Pay On Use Return", 93, StampsClient.v111.ServiceType.USRETURN)
        {
        }
    }
}
