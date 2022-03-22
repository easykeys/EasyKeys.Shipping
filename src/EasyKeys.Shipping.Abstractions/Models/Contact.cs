namespace EasyKeys.Shipping.Abstractions.Models
{
    public class Contact
    {
        /// <summary>
        /// first and last name.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Determines if address is residential or commercial.
        /// </summary>
        public string ResidentialIndicator { get; set; } = string.Empty;

        public string AccountNumber { get; set; } = string.Empty;
    }
}
