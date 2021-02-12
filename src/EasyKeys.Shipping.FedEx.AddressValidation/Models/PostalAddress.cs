namespace EasyKeys.Shipping.FedEx.AddressValidation.Models
{
    public class PostalAddress
    {
        public string Address { get; set; } = string.Empty;

        public string Address2 { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string StateOrProvince { get; set; } = string.Empty;

        public string PostalCode { get; set; } = string.Empty;

        public string CountryCode { get; set; } = string.Empty;

        public bool IsResidential { get; set; }
    }
}
