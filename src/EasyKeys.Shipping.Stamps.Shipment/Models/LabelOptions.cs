namespace EasyKeys.Shipping.Stamps.Shipment.Models
{
    public class LabelOptions
    {
        /// <summary>
        /// Specifies the information to enable the system to send the label URL via email. Currently this feature is only available for return labels.
        /// </summary>
        public EmailLabelTo EmailLabelTo { get; set; } = new EmailLabelTo();

        /// <summary>
        /// Determines whether the image URL or the actual image data will be returned in the response object.
        /// * If ReturnImageData is false, one or more image URL will be returned in the URL element in the response object.
        /// * Each URL will need to be queried to retrieve the actual image.
        /// * If ReturnImageData is true, the actual image data will be returned as a base64 string in the ImageData object.
        /// * The URL element will be an empty string.
        /// * This mode cannot be used on ImageType of Auto, PrintOncePDF, or EncryptedPngURL.
        /// </summary>
        public bool ReturnImageData { get; set; } = true;

        /// <summary>
        /// Normal : A regular label with postage and a valid indicium (<b>default</b>).
        /// NoPostage : A regular label without postage or an indicium.
        /// </summary>
        public PostageModeType PostageMode { get; set; } = PostageModeType.Normal;

        /// <summary>
        /// Image type of shipping label. Default is Auto: which generates a Png image.
        /// </summary>
        public ImageType ImageType { get; set; } = ImageType.Png;

        /// <summary>
        /// The memo to print at the bottom of the shipping label. The memo parameter may consist of more than one line separated by the standard carriage return/line feed, use &#xd; as carriage return and &#xa; as line feed in the request.
        /// </summary>
        public string Memo { get; set; } = "testing memo";

        /// <summary>
        /// Specifies the page size of PDF labels. This value only applies to PDF. If offset is specified, this value will be ignored.
        /// <see href="https://developer.stamps.com/soap-api/reference/swsimv111.html#carrier">more documentation</see>.
        /// <list type="bullet">
        /// <item>
        /// <description>default</description>
        /// </item>
        /// <item>
        /// <description>LabelSize</description>
        /// </item>
        /// <item>
        /// <description>Letter85x11</description>
        /// </item>
        /// </list>
        /// </summary>
        public PaperSizeType PaperSize { get; set; } = PaperSizeType.Default;

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
        public DpiType DpiType { get; set; } = DpiType.Default;

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
        public ImageDpiType ImageDPI { get; set; } = ImageDpiType.Default;
    }
}
