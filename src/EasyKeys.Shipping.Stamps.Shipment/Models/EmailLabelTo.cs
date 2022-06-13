namespace EasyKeys.Shipping.Stamps.Shipment.Models
{
    public class EmailLabelTo
    {
        /// <summary>
        /// Activate label emailing.
        /// </summary>
        public bool IsActivated { get; set; } = true;

        /// <summary>
        /// The email address that receives the label.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Determines whether the email needs to be copied to the email address of the account.
        /// </summary>
        public bool CopyToOriginator { get; set; } = false;

        /// <summary>
        /// Name of the person who will receive the email.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Customized text that goes in the email body.
        /// </summary>
        public string Note { get; set; } = string.Empty;
    }
}
