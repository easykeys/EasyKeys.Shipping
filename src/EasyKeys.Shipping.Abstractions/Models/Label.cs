namespace EasyKeys.Shipping.Abstractions.Models
{
    public class Label
    {
        public string TrackingNumber { get; set; } = string.Empty;

        /// <summary>
        /// Charge by the provider to customer.
        /// </summary>
        public decimal BaseCharge { get; set; }

        /// <summary>
        /// Actual charge to the shipper based on negotioated discount.
        /// </summary>
        public decimal NetCharge { get; set; }

        public decimal TotalSurcharges { get; set; }

        public List<LabelDetails> LabelDetails { get; set; } = new List<LabelDetails>();

        /// <summary>
        ///     Internal library errors during interaction with service provider
        ///     (e.g. SoapException was thrown).
        /// </summary>
        public List<string> InternalErrors { get; } = new List<string>();
    }
}
