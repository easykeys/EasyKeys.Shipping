namespace EasyKeys.Shipping.Abstractions.Models
{
    public class LabelOptions
    {
        /// <summary>
        /// Required for FedEx Shipping : COMMON2D, LABEL_DATA_ONLY.
        /// </summary>
        public string LabelFormatType { get; set; } = String.Empty;

        /// <summary>
        /// pdf, png, ect.
        /// </summary>
        public string ImageType { get; set; } = String.Empty;

        /// <summary>
        /// size of label, default to 4x6.
        /// </summary>
        public string LabelSize { get; set; } = "4x6";

        /// <summary>
        /// shipper information on label.
        /// </summary>
        public Contact Shipper { get; set; } = new Contact();

        /// <summary>
        /// recipient information on label.
        /// </summary>
        public Contact Recipient { get; set; } = new Contact();

        /// <summary>
        /// Default payment type would be sender unless its a COD.
        /// </summary>
        public string PaymentType { get; set; } = "Sender";

        /// <summary>
        /// Collect on delivery is defaulted to false.
        /// </summary>
        public CollectOnDelivery CollectOnDelivery { get; set; } = new CollectOnDelivery();
    }
}
