namespace EasyKeys.Shipping.Abstractions.Models;

public class ShipmentLabel
{
    /// <summary>
    /// (non-label) shipping documents produced with a specific shipment.Commercial Invoice automatically generated for international shipments.
    /// </summary>
    public IList<Document> ShippingDocuments { get; set; } = new List<Document>();

    /// <summary>
    /// Labels for the <see cref="Package"/>.
    /// </summary>
    public IList<PackageLabelDetails> Labels { get; set; } = new List<PackageLabelDetails>();

    /// <summary>
    ///     Internal library errors during interaction with service provider
    ///     (e.g. SoapException was thrown).
    /// </summary>
    public IList<string> InternalErrors { get; } = new List<string>();
}
