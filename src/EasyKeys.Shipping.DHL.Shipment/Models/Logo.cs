namespace EasyKeys.Shipping.DHL.Shipment.Models;

public class Logo
{
    /// <summary>
    /// base64 encoded string of the logo.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 0 = png, 1 = gif, 2 = jpeg, 3 = jpg.
    /// </summary>
    public int FileFormat { get; set; }
}
