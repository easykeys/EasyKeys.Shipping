namespace EasyKeys.Shipping.FedEx.AddressValidation.Models
{
    public class ValidationResponse
    {
        public PostalAddress OriginalAddress { get; set; } = new PostalAddress();

        public PostalAddress ProposedAddress { get; set; } = new PostalAddress();

        public bool IsConfirmedBusiness { get; set; }

        public int Score { get; set; }

        public string Changes { get; set; } = string.Empty;

        public string ResidentialStatus { get; set; } = string.Empty;

        public string DeliveryPointValidation { get; set; } = string.Empty;

        public bool IsValid { get; set; }

        public string Notes { get; set; } = string.Empty;

        public string ValidationErrors { get; set; } = string.Empty;
    }
}
