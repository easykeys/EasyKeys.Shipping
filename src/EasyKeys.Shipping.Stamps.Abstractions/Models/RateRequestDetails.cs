﻿namespace EasyKeys.Shipping.Stamps.Abstractions.Models
{
    public class RateRequestDetails
    {

        /// <summary>
        /// Required for CreateIndicium web method.
        /// Not required for GetRates web method.If not specified in GetRates, <b>rate</b> object in the response will include rates for all ServiceTypes.
        /// </summary>
        public ServiceType ServiceType { get; set; } = ServiceType.UNKNOWN;

        /// <summary>
        /// A plain language description of the service returned, i.e. “USPS Priority Mail”.
        /// </summary>
        public string ServiceDescription { get; set; } = string.Empty;

        /// <summary>
        /// The amount to insure this shipment for, in dollars and cents. If a value is specified, insurance add-ons will be returned.
        /// </summary>
        public decimal InsuredValue { get; set; } = 100.0m;

        /// <summary>
        /// The registered value for this shipment, in dollars and cents.
        /// </summary>
        public decimal RegisteredValue { get; set; }

        /// <summary>
        /// The amount to declare for this shipment, in dollars and cents. Required for International.
        /// </summary>
        public decimal DeclaredValue { get; set; }

        /// <summary>
        /// 	Default: false This is used to calculate a potential additional handling surcharge for certain types of items being sent. See DMM 401 for what qualifies.
        /// </summary>
        public bool NonMachinable { get; set; } = false;

        /// <summary>
        /// Default: true Represents whether or not the parcel being sent is rectangular-shaped, or box-like. For dim-weight-based rates (Priority Mail), non-rectangular-shaped parcels may lower the effective weight.
        /// </summary>
        public bool RectangularShaped { get; set; } = true;

        /// <summary>
        /// List of items prohibited from mailing based on country of destination. Only used for International.
        /// </summary>
        public string Prohibitions { get; set; } = string.Empty;

        /// <summary>
        /// Restrictions on items being shipped based on country of destination. Only used for International.
        /// </summary>
        public string Restrictions { get; set; } = string.Empty;

        /// <summary>
        /// Notes specific to Priority Mail Express International service. Only used for international when ServiceType is US-GEM.
        /// </summary>
        public string GEMNotes { get; set; } = string.Empty;

        /// <summary>
        /// Only returned for international mail classes, this field explains the maximum package dimensions for this mail class and destination country.
        /// </summary>
        public string MaxDimensions { get; set; } = string.Empty;

        /// <summary>
        /// If a <b>rate</b> object is returned from a GetRates call with DimWeighting = "Y", this indicates that the dimensions of the package may have an impact on the rate. Be sure to also include the Length, Width and Height for proper rating whenever DimWeighting = "Y".
        /// </summary>
        public string DimWeighting { get; set; } = string.Empty;

        /// <summary>
        /// Additional mailing information based on country of destination. Only used for International.
        /// </summary>
        public string Observations { get; set; } = string.Empty;

        /// <summary>
        /// Additional regulations for shipping to destination country. Only used for International.
        /// </summary>
        public string Regulations { get; set; } = string.Empty;

        /// <summary>
        /// <para>
        ///  Cubic Pricing is available only for Priority Mail with PackageType “Large Envelope or Flat”, “Package”, “Large Package” or “Oversized Package”.
        ///  If PackageType in <b>rate</b> object is “Large Envelope or Flat”, Length and Width are prerequisites to determine eligibility for cubic pricing.
        ///  If Length and Width were provided, and Height is less than or equal to ¾”, and a <b>rate</b> object is returned from a GetRates call with CubicPricing = "Y",
        ///  it indicates that the dimensions of the package qualifies the package for soft-pack or padded envelope cubic pricing. If Length and Width were provided,
        ///  and Height is greater than ¾”, and a <b>rate</b> object is returned from a GetRates call with CubicPricing = "Y",
        ///  it indicates that the dimensions of the package qualifies the package for parcel cubic pricing. If Length and Width were not provided,
        ///  and a <b>rate</b> object is returned from a GetRates call with CubicPricing = "Y",
        ///  it indicates that the dimensions of the package may qualify the package for soft-pack or padded envelope cubic pricing or parcel cubic pricing.
        ///  If PackageType of <b>rate</b> object is “Package”, “Large Package” or “Oversized Package”, Length, Width and Height are prerequisites to determine eligibility for parcel cubic pricing.
        ///  If Length, Width and Height were provided, and a <b>rate</b> object is returned from a GetRates call with CubicPricing = "Y",
        ///  it indicates that the dimensions of the package qualifies the package for parcel cubic pricing. If Length, Width and Height were not provided,
        ///  and a <b>rate</b> object is returned from a GetRates call with CubicPricing = "Y", it indicates that the dimensions of the package may qualify the package for parcel cubic pricing.
        /// </para>
        /// </summary>
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

        /// <summary>
        /// <para>
        /// When PackageType is submitted, the GetRates web method will respond with the rate based on the specified package.
        /// However, if PackageType is omitted, the GetRates web method will respond with rates for all of the possible package types for the given ServiceType.
        /// PackageType is required for CreateIndicium web method.
        /// </para>
        /// </summary>
        public PackageType PackageType { get; set; } = PackageType.Package;

        public decimal CODValue { get; set; }
    }
}