namespace EasyKeys.Shipping.FedEx.Shipment.Models;

public class LabelOptions
{
    /// <summary>
    /// COMMON2D, LABEL_DATA_ONLY.
    /// </summary>
    public string LabelFormatType { get; set; } = "COMMON2D";

    /// <summary>
    /// pdf, png are universal for all providers.
    /// </summary>
    public string ImageType { get; set; } = "PNG";

    /// <summary>
    /// size of label, default to 4x6.
    /// "4x675","4x8","Paper_Letter".
    /// </summary>
    public string LabelSize { get; set; } = "4x6";

    public bool EnableEtd { get; set; } = true;

    public int LetterHeadImageId { get; set; } = 0;

    public int SignatureImageId { get; set; } = 1;
}
