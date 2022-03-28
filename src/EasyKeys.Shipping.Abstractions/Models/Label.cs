namespace EasyKeys.Shipping.Abstractions.Models
{
    public class Label
    {
        public Charges TotalCharges { get; set; } = new Charges();

        public string MasterTrackingNumber { get; set; } = string.Empty;

        public List<LabelDetails> LabelDetails { get; set; } = new List<LabelDetails>();

        /// <summary>
        ///     Internal library errors during interaction with service provider
        ///     (e.g. SoapException was thrown).
        /// </summary>
        public List<string> InternalErrors { get; } = new List<string>();
    }
}
