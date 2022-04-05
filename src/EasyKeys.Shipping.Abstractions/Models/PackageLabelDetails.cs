namespace EasyKeys.Shipping.Abstractions.Models;

public class PackageLabelDetails
{
    public PackageCharges Charges { get; set; } = new PackageCharges();

    public string TrackingId { get; set; } = string.Empty;

    public string ImageType { get; set; } = string.Empty;

    public List<byte[]> Bytes { get; set; } = new List<byte[]>();
}
