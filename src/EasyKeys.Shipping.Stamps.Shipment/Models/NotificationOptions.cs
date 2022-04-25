namespace EasyKeys.Shipping.Stamps.Shipment.Models
{
    public class NotificationOptions
    {
        /// <summary>
        /// Activate email notifications on shipment.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// E-mail address for recipient of a shipment notification e-mail.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Set to true to send a copy of the shipment notification e-mail, if any, back to the e-mail address associated with the Stamps.com account.
        /// </summary>
        public bool CC_ToAccountHolder { get; set; } = true;

        /// <summary>
        /// Set to true to use the company name of the Stamps.com account, instead of the first and last name, in the From line of the shipment notification e-mail, if any.
        /// </summary>
        public bool UseCompanyNameInFromLine { get; set; } = true;

        /// <summary>
        /// Set to true to use the company name of the Stamps.com account, instead of the first and last name, in the Subject line of the shipment notification e-mail, if any.
        /// </summary>
        public bool UseCompanyNameInSubject { get; set; } = true;
    }
}
