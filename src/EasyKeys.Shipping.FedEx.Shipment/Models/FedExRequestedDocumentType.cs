using Ardalis.SmartEnum;

using ShipClient.v25;

namespace EasyKeys.Shipping.FedEx.Shipment.Models
{
    public abstract class FedExRequestedDocumentType : SmartEnum<FedExRequestedDocumentType>
    {
        public static readonly FedExRequestedDocumentType CommercialInvoice = new CommercialInvoiceType();
        public static readonly FedExRequestedDocumentType ProFormaInvoice = new ProFormaInvoiceType();
        public static readonly FedExRequestedDocumentType CertificateOfOrigin = new CertificateOfOriginType();

        protected FedExRequestedDocumentType(string name, int value) : base(name, value)
        {
        }

        private class CommercialInvoiceType : FedExRequestedDocumentType
        {
            public CommercialInvoiceType() : base(RequestedShippingDocumentType.COMMERCIAL_INVOICE.ToString(), (int)RequestedShippingDocumentType.COMMERCIAL_INVOICE)
            {
            }
        }

        private class ProFormaInvoiceType : FedExRequestedDocumentType
        {
            public ProFormaInvoiceType() : base(RequestedShippingDocumentType.PRO_FORMA_INVOICE.ToString(), (int)RequestedShippingDocumentType.PRO_FORMA_INVOICE)
            {
            }
        }

        private class CertificateOfOriginType : FedExRequestedDocumentType
        {
            public CertificateOfOriginType() : base(RequestedShippingDocumentType.CERTIFICATE_OF_ORIGIN.ToString(), (int)RequestedShippingDocumentType.CERTIFICATE_OF_ORIGIN)
            {
            }
        }
    }
}
