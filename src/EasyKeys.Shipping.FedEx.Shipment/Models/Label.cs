using ShipClient.v25;

namespace EasyKeys.Shipping.FedEx.Shipment
{
    public class Label
    {
        public ShippingDocumentImageType ShippingDocumentImageType { get; set; }

        public byte[] Bytes { get; set; }

        /// <summary>
        ///     Internal library errors during interaction with service provider
        ///     (e.g. SoapException was thrown).
        /// </summary>
        public List<string> InternalErrors { get; } = new List<string>();
    }
}
