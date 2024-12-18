using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Amazon.Shipment.Models;

public class ShippingDetails
{
    public string ReferenceId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Sender Contact Info.
    /// </summary>
    public ContactInfo Sender { get; set; } = new ContactInfo();

    /// <summary>
    /// Recipient Contact Info.
    /// </summary>
    public ContactInfo Recipient { get; set; } = new ContactInfo();

    public string LabelFormat { get; set; } = "PNG";

    public Dimensions LabelDimensions { get; set; } = new Dimensions
    {
        Length = 6,
        Width = 4
    };

    public string LabelUnit { get; set; } = "INCH";

    public int LabelDpi { get; set; } = 300;

    public string ServiceId { get; set; } = "std-us-swa-mfn";
}
