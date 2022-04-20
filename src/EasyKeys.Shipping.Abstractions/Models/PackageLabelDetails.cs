namespace EasyKeys.Shipping.Abstractions.Models;

public class PackageLabelDetails
{
    public PackageCharges Charges { get; set; } = new PackageCharges();

    /// <summary>
    /// special id given by provider for cancellation purposes.
    /// ex.StampsTxID.
    /// </summary>
    public string ProviderLabelId { get; set; } = string.Empty;

    public string TrackingId { get; set; } = string.Empty;

    public string ImageType { get; set; } = string.Empty;

    public List<byte[]> Bytes { get; set; } = new List<byte[]>();
}
