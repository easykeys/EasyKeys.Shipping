namespace EasyKeys.Shipping.DHL.Shipment.Models;

public class Signature
{
    /// <summary>
    /// base64 encoded string of the signature.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// signature name.
    /// </summary>
    public string SignatureName { get; set; } = string.Empty;

    /// <summary>
    /// signature title.
    /// </summary>
    public string SignatureTitle { get; set; } = string.Empty;
}
