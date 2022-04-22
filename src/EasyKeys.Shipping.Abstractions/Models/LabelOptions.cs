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

    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// default
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// high
    /// </description>
    /// </item>
    /// </list>
    /// </summary>
    public string Resolution { get; set; } = "default";

    /// <summary>
    /// The memo to print at the bottom of the shipping label. The memo parameter may consist of more than one line separated by the standard carriage return/line feed, use &#xd; as carriage return and &#xa; as line feed in the request.
    /// </summary>
    public string Memo { get; set; } = string.Empty;

    /// <summary>
    /// <list type="bullet">
    /// <listheader>
    /// <term>Stamps.com</term>
    /// <description>
    /// Represent the number of degrees of counter-clockwise rotation for the label.
    /// </description>
    /// </listheader>
    /// <item>
    /// <description>0</description>
    /// </item>
    /// <item>
    /// <description>90</description>
    /// </item>
    /// <item>
    /// <description>180</description>
    /// </item>
    /// <item>
    /// <description>270</description>
    /// </item>
    /// </list>
    /// </summary>
    public int RotationDegrees { get; set; }

    /// <summary>
    /// Indicates how many units the label should be offset on the x-axis. Applies only to thermal printers.
    /// </summary>
    public int HorizontalOffet { get; set; }

    /// <summary>
    /// Indicates how many units the label should be offset on the y-axis. Applies only to thermal printers.
    /// </summary>
    public int VerticalOffet { get; set; }

    /// <summary>
    /// Density settings for thermal printers, to help control the darkness of the print.
    /// </summary>
    public int PrintDensity { get; set; }

    /// <summary>
    /// This applies to domestic and international, combined and separate CP72 and CN22 layouts.
    /// For other label types this parameter can be specified but is ignored.
    /// If the parameter is omitted, or is set to “true”, instructions will be included as part of the label.
    /// </summary>
    public bool PrintInstructions { get; set; } = false;

    /// <summary>
    /// <term>Stamps.com</term>
    /// <list type="bullet">
    /// <item>
    /// <term>ImageDpi300</term>
    /// <description>High resolution, valid for printing postage</description>
    /// <description>
    /// </description>
    /// </item>
    /// <item>
    /// <term>ImageDpi203</term>
    /// <description>Default DPI, for use with thermal printers</description>
    /// <description>
    /// </description>
    /// </item>
    /// <item>
    /// <term>ImageDpi200</term>
    /// <description>Low resolution, valid for printing postage</description>
    /// <description>
    /// </description>
    /// </item>
    /// <item>
    /// <term>ImageDpi150</term>
    /// <description>Low resolution, valid for printing postage</description>
    /// <description>
    /// </description>
    /// </item>
    /// <item>
    /// <term>ImageDpi96</term>
    /// <description> not-valid for printing postage, for sample use only</description>
    /// <description>
    /// </description>
    /// </item>
    /// </list>
    /// </summary>
    public string ImageDPI { get; set; } = "ImageDpi203";

    /// <summary>
    /// Specifies the information to enable the system to send the label URL via email.
    /// Currently this feature is only available for return labels.
    /// </summary>
    public string EmailLabelTo { get; set; } = String.Empty;
}
