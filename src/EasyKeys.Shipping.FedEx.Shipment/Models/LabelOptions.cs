using EasyKeys.Shipping.FedEx.Rates;

using ShipClient.v25;

namespace EasyKeys.Shipping.FedEx.Shipment
{
    public class LabelOptions
    {
        public DropoffType DropoffType { get; set; }

        /// <summary>
        /// ServiceType cannot equal default.
        /// </summary>
        public ServiceType ServiceType { get; set; }

        public FedExPackageType PackageType { get; set; }

        public WeightUnits Units { get; set; } = WeightUnits.LB;

        /// <summary>
        /// shipper contact information.
        /// </summary>
        public Contact ShipperContact { get; set; }

        public Contact RecipientContact { get; set; }

        public bool IsCodShipment { get; set; } = false;

        public CodDetail CodDetail { get; set; } = new CodDetail();

        public PaymentType PaymentType { get; set; }

        /// <summary>
        /// If PaymentType is set to RECIPIENT or THIRD PARTY, then Contact and Address must both
        /// be populated. Required fields are either the company/person name and the
        /// phone number. Required address fields are the street line 1, city, state/provice
        /// code and country code.
        /// </summary>
        public Party ResponsibleParty { get; set; } = new Party();

        /// <summary>
        /// If this is a CodShipment, user Account number is needed.
        /// </summary>
        public string AccountNumber { get; set; } = string.Empty;

        /// <summary>
        /// Required to receive the correct label image in the Ship Reply service.
        /// Valid values:
        /// • COMMON2D — label format type to receive a label.
        /// • LABEL_DATA_ONLY — this value is used to receive the barcode data if you create a custom label.
        /// </summary>
        public LabelFormatType LabelFormatType { get; set; }

        /// <summary>
        /// THERMAL PRINTERS:
        /// Optional for each label request, however required to print the labels requested.
        /// Valid values are:
        /// • PAPER_4X6
        /// • PAPER_4X6.75
        /// • PAPER_4X8
        /// • PAPER_4X9
        /// • PAPER_7X4.75
        /// • PAPER_8.5X11_BOTTOM_HALF_LABEL
        /// • PAPER_8.5X11_TOP_HALF_LABEL
        /// • STOCK_4X6
        /// • STOCK_4X6.75
        /// • STOCK_4X6.75_LEADING_DOC_TAB
        /// • STOCK_4X6.75_TRAILING_DOC_TAB
        /// • STOCK_4X8
        /// • STOCK_4X9
        /// • STOCK_4X9_LEADING_DOC_TAB
        /// • STOCK_4X9_TRAILING_DOC_TAB.
        /// Note: STOCK_4X6 and STOCK_4X8 are designed for
        /// both thermal labels and pdf labels to be printed on a
        /// thermal printer.
        ///
        /// LASER PRINTERS:
        /// Required for all label types.
        /// When using an ImageType of PDF or PNG
        /// • these values display a thermal format label:
        /// o PAPER_4X6
        /// o PAPER_4X8
        /// o PAPER_4X9
        /// • These values display a plain paper format shipping
        /// label:
        /// o PAPER_7X4.75
        /// o PAPER_8.5X11_BOTTOM_HALF_LABEL
        /// o PAPER_8.5X11_TOP_HALF_LABEL
        /// </summary>

        public LabelStockType LabelStockType { get; set; }

        /// <summary>
        /// Required to indicate label formatting. Type of data stream or bitmap to be returned:
        /// Valid values are:
        /// • PDF — plain paper
        /// • PNG — plain paper
        /// • DOC
        /// • RTF
        /// • TEXT
        /// • EPL2
        /// • ZPLII.
        /// </summary>

        public ShippingDocumentImageType ShippingDocumentImageType { get; set; }

        /// <summary>
        /// fulfillment contact and address information that will be on label.
        /// </summary>
        public ContactAndAddress FulfillmentContactAndAddress { get; set; }

        public SignatureOptionType SignatureOptionType { get; set; }
    }
}
