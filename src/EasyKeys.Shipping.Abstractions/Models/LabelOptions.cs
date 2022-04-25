namespace EasyKeys.Shipping.Abstractions.Models;

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
    /// <list type="bullet">
    ///    <listheader>
    ///    <term>Fedex Options</term>
    ///    <description>description</description>
    ///    <see href="https://developer.fedex.com/api/en-us/home.html">more documentation</see>
    ///     </listheader>
    /// <item>
    /// <description>default = "4x6"</description>
    /// </item>
    /// <item>
    /// <description>4x675</description>
    /// </item>
    /// <item>
    /// <description>4x8</description>
    /// </item>
    /// <item>
    /// <description>Paper_Letter</description>
    /// </item>
    /// </list>
    /// <list type="bullet">
    ///    <listheader>
    ///    <term>Stamps.com Options</term>
    /// <description>See documentation for more options</description>
    ///    <see href="https://developer.stamps.com/soap-api/reference/swsimv111.html#carrier">more documentation</see>
    ///     </listheader>
    /// <item>
    /// <description>Normal or no value = "4X6"</description>
    /// </item>
    /// <item>
    /// <description>NormalLeft</description>
    /// </item>
    /// <item>
    /// <description>NormalRight</description>
    /// </item>
    /// <item>
    /// <description>Normal4X6</description>
    /// </item>
    /// <item>
    /// <description>4X45</description>
    /// </item>
    /// </list>
    /// </summary>
    public string LabelSize { get; set; } = "4x6";
}
