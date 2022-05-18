using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.Shipment.Models
{
    public class ShipmentRequestDetails
    {
        public CustomsInformation CustomsInformation { get; set; } = new CustomsInformation();

        public LabelOptions LabelOptions { get; set; } = new LabelOptions();

        public NotificationOptions NotificationOptions { get; set; } = new NotificationOptions();

        public Rate SelectedRate { get; set; }

        public RateRequestDetails RateRequestDetails { get; set; } = new RateRequestDetails();

        public PackageType PackageType { get; set; } = PackageType.Package;

        /// <summary>
        /// <list type="bullet">
        ///    <listheader>
        ///    <term>Fedex Options</term>
        ///    <description>description</description>
        ///    <see href="https://developer.stamps.com/soap-api/reference/swsimv111.html#getratesresponse-object">more documentation</see>
        ///     </listheader>
        /// <item><description>default = Merchandise</description></item>
        /// <item> <description>Commercial_Sample</description></item>
        /// <item><description>Dangerous_Goods</description></item>
        /// <item><description>Document</description></item>
        /// <item><description>Gift</description> </item>
        /// <item><description>Humanitarian_Donation</description></item>
        /// <item><description>Returned_Goods</description> </item>
        /// <item><description>Other</description></item>
        /// </list>
        /// </summary>
        public ContentType ContentType { get; set; } = ContentType.Merchandise;

        public bool ContentTypeSpecified { get; set; } = true;

        /// <summary>
        /// The amount to declare for this shipment, in dollars and cents. Required for International.
        /// </summary>
        public decimal DeclaredValue { get; set; }

        /// <summary>
        /// Generates a sample label. Default is <b>true</b>.
        /// </summary>
        public bool IsSample { get; set; } = true;

        public string OrderId { get; set; } = string.Empty;
    }
}
