using ShipClient.v25;

namespace EasyKeys.Shipping.FedEx.Shipment
{
    public class Label
    {
        public ShippingDocumentImageType ShippingDocumentImageType { get; set; }

        public byte[] Bytes { get; set; }
    }
}
