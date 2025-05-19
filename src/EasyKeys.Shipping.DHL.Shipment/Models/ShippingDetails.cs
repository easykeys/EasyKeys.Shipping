using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.DHL.Shipment.Models;

public class ShippingDetails
{
    /// <summary>
    /// Sender Contact Info.
    /// </summary>
    public ContactInfo Sender { get; set; } = new ContactInfo();

    /// <summary>
    /// Recipient Contact Info.
    /// </summary>
    public ContactInfo Recipient { get; set; } = new ContactInfo();

    /// <summary>
    /// Required for International Shipments Only.
    /// A collection shipment contents that are considered to be dutiable.
    /// </summary>
    public IList<Commodity> Commodities { get; } = new List<Commodity>();

    /// <summary>
    /// product code returned from rates/products api.
    /// </summary>
    public string ProductCode { get; set; } = string.Empty;

    public List<Logo> Logos { get; set; } = new List<Logo>();

    /// <summary>
    /// signature for the invoice.
    /// </summary>
    public Signature? Signature { get; set; }

    /// <summary>   
    /// key : II insurance, WY paperless.,SF direct signature.
    /// value : amount if required.
    /// </summary>
    public Dictionary<string, double?> AddedServices { get; set; } = new Dictionary<string, double?>();

    /// <summary>
    /// Gets or sets the unique identifier for the invoice.
    /// </summary>
    public string InvoiceNumber { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets a custom message to include with the shipment.
    /// </summary>
    public string? CustomShipmentMessage { get; set; }

    /// <summary>
    /// Gets or sets the description of the package.
    /// </summary>
    public string? PackageDescription { get; set; }

    /// <summary>
    /// Gets or sets the description associated with the label.
    /// </summary>
    public string? LabelDescription { get; set; }

    public bool SplitTransportAndWaybillDocLabls { get; set; } = false;

    public bool AllDocumentsInOneImage { get; set; } = false;

    public bool SplitDocumentsByPages { get; set; } = false;

    public bool SplitInvoiceAndReceipt { get; set; } = true;

    public bool ReceiptAndLabelsInOneImage { get; set; } = false;

    public bool IsPickupRequested { get; set; } = false;
}
