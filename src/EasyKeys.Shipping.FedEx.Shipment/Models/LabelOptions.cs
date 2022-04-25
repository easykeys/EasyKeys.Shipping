namespace EasyKeys.Shipping.FedEx.Shipment.Models;

public class LabelOptions
{
    /// <summary>
    /// Required for FedEx Shipping : COMMON2D, LABEL_DATA_ONLY.
    /// </summary>
    public string LabelFormatType { get; set; } = string.Empty;

    /// <summary>
    /// pdf, png are universal for all providers.
    /// </summary>
    public string ImageType { get; set; } = string.Empty;

    /// <summary>
    /// size of label, default to 4x6.
    /// "4x675","4x8","Paper_Letter".
    /// </summary>
    public string LabelSize { get; set; } = "4x6";
}
