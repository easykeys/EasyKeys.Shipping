namespace EasyKeys.Shipping.Stamps.Abstractions.Models
{
    public class RateRequestDetails
    {

        /// <summary>
        /// Default to  unknown.
        /// </summary>
        public ServiceType ServiceType { get; set; } = ServiceType.UNKNOWN;

        /// <summary>
        /// A plain language description of the service returned, i.e. “USPS Priority Mail”.
        /// </summary>
        public string ServiceDescription { get; set; } = string.Empty;

        public decimal InsuredValue { get; set; } = 100.0m;

        public decimal RegisteredValue { get; set; }

        public decimal DeclaredValue { get; set; }

        public bool NonMachinable { get; set; } = false;

        public bool RectangularShaped { get; set; } = true;

        public string Prohibitions { get; set; } = string.Empty;

        public string Restrictions { get; set; } = string.Empty;

        public string GEMNotes { get; set; } = string.Empty;

        public string MaxDimensions { get; set; } = string.Empty;

        public string DimWeighting { get; set; } = string.Empty;

        public string Observations { get; set; } = string.Empty;

        public string Regulations { get; set; } = string.Empty;

        public int Zone { get; set; } = 0;

        public int RateCategory { get; set; } = 0;

        public bool CubicPricing { get; set; } = false;

        /// <summary>
        /// <list type="bullet">
        ///    <listheader>
        ///    <term>Fedex Options</term>
        ///    <description>description</description>
        ///    <see href="https://developer.stamps.com/soap-api/reference/swsimv111.html#getratesresponse-object">more documentation</see>
        ///     </listheader>
        /// <item>
        /// <description>default = Merchandise</description>
        /// </item>
        /// <item>
        /// <description>Commercial_Sample</description>
        /// </item>
        /// <item>
        /// <description>Dangerous_Goods</description>
        /// </item>
        /// <item>
        /// <description>Document</description>
        /// </item>
        /// <item>
        /// <description>Gift</description>
        /// </item>
        /// <item>
        /// <description>Humanitarian_Donation</description>
        /// </item>
        /// <item>
        /// <description>Returned_Goods</description>
        /// </item>
        /// <item>
        /// <description>Other</description>
        /// </item>
        /// </list>
        /// One of: Commercial_Sample, Dangerous_Goods, Document, Gift, Humanitarian_Donation, Merchandise, Returned_Goods, or Other.
        /// </summary>
        public string ContentType { get; set; } = "Merchandise";

        public bool ContentTypeSpecified { get; set; } = true;

        /// <summary>
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// USPS
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// FedEx
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// UPS
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// DHLExpress
        /// </description>
        /// </item>
        /// </list>
        /// </summary>
        public string Carrier { get; set; } = "USPS";

        public string PackageType { get; set; } = string.Empty;

        public decimal CODValue { get; set; }
    }
}
