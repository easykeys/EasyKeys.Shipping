namespace EasyKeys.Shipping.Abstractions.Models;

public class PackageLabelDetails
{
    /// <summary>
    /// The total charge for a sender.
    /// </summary>
    public ShipmentCharges TotalCharges { get; set; } = new ShipmentCharges();

    /// <summary>
    /// The total charge for a recipient.
    /// </summary>
    public ShipmentCharges TotalCharges2 { get; set; } = new ShipmentCharges();

    /// <summary>
    /// special id given by provider for cancellation purposes.
    /// ex.StampsTxID.
    /// </summary>
    public string ProviderLabelId { get; set; } = string.Empty;

    /// <summary>
    /// Tracking Ids, seperated by ';'.
    /// </summary>
    public string TrackingId { get; set; } = string.Empty;

    /// <summary>
    /// Type of the Image.
    /// </summary>
    public string ImageType { get; set; } = string.Empty;

    /// <summary>
    /// If multiple files.
    /// </summary>
    public List<byte[]>? Bytes { get; set; } = new List<byte[]>();
}
