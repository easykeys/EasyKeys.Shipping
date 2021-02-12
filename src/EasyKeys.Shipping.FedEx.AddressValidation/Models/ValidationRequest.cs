namespace EasyKeys.Shipping.FedEx.AddressValidation.Models
{
    public class ValidationRequest
    {
        public PostalAddress Address { get; set; } = new PostalAddress();
    }
}
