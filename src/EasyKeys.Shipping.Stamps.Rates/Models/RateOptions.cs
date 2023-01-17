using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.Rates.Models;

/// <summary>
/// Stamps.com configure rates request.
/// </summary>
public class RateOptions
{
    public ContactInfo Sender { get; set; } = new ContactInfo();

    public ContactInfo Recipient { get; set; } = new ContactInfo();

    /// <summary>
    /// Required for CreateIndicium web method.
    /// Not required for GetRates web method.If not specified in GetRates, <b>rate</b> object in the response will include rates for all ServiceTypes.
    /// </summary>
    public StampsServiceType ServiceType { get; set; } = StampsServiceType.Unknown;

    /// <summary>
    /// The registered value for this shipment, in dollars and cents.
    /// </summary>
    public decimal RegisteredValue { get; set; }

    /// <summary>
    ///     Default: false This is used to calculate a potential additional handling surcharge for certain types of items being sent.
    ///     See DMM 401 for what qualifies.
    /// </summary>
    public bool NonMachinable { get; set; }

    /// <summary>
    /// Default: true Represents whether or not the parcel being sent is rectangular-shaped, or box-like.
    /// For dim-weight-based rates (Priority Mail), non-rectangular-shaped parcels may lower the effective weight.
    /// </summary>
    public bool RectangularShaped { get; set; } = true;

    /// <summary>
    /// If a <b>rate</b> object is returned from a GetRates call with DimWeighting = "Y", this indicates that the dimensions of the package may have an impact on the rate. Be sure to also include the Length, Width and Height for proper rating whenever DimWeighting = "Y".
    /// </summary>
    public string DimWeighting { get; set; } = string.Empty;

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
    public StampsCarrierType Carrier { get; set; } = StampsCarrierType.Usps;

    /// <summary>
    /// The collect on delivery value for this shipment, in dollars and cents.
    /// </summary>
    public decimal CODValue { get; set; }

    /// <summary>
    /// <para>
    /// Format: Decimal with up to three digits to the right of the decimal point This is the rate cost for the particular shipment option.
    /// If this is blank or missing, check the error attribute for a reason.
    /// If there is not enough information in the input Rate object to calculate an exact amount,
    /// Amount will be set to the lower bound of the known range and MaxAmount will be set to the upper bound.
    /// For example, if the ToZipCode is omitted and the Rate is for a zone-based service,
    /// Amount will be the amount for the closest zone (1) and MaxAmount will be the amount for the furthest zone (8).
    /// In this case, some other elements of the Rate object may not apply in all cases.
    /// For example, when Amount and MaxAmount indicate a range of possible amounts, DimWeighting may only apply for the furthest zones (5-8).
    /// </para>
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// <para>
    /// Format: Decimal with up to three digits to the right of the decimal point This is the maximum rate cost for the particular shipment option.
    /// When the exact rate can be calculated, MaxAmount will be equal to Amount.
    /// If there is not enough information in the input Rate object to calculate an exact amount,
    /// Amount will be set to the lower bound of the known range and MaxAmount will be set to the upper bound.
    /// For example, if the ToZipCode is omitted and the Rate is for a zone-based service,
    /// Amount will be the amount for the closest zone (1) and MaxAmount will be the amount for the furthest zone (8).
    /// In this case, some other elements of the Rate object may not apply in all cases.
    /// For example, when Amount and MaxAmount indicate a range of possible amounts, DimWeighting may only apply for the furthest zones (5-8).
    /// </para>
    /// </summary>
    public decimal MaxAmount { get; set; }

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
    public StampsContentType ContentType { get; set; } = StampsContentType.Merchandise;

    /// <summary>
    /// The amount to declare for this shipment, in dollars and cents. Required for International.
    /// </summary>
    public decimal DeclaredValue { get; set; }

    /// <summary>
    /// Additional mailing information based on country of destination. Only used for International.
    /// </summary>
    public string Observations { get; set; } = string.Empty;

    /// <summary>
    /// Additional regulations for shipping to destination country. Only used for International.
    /// </summary>
    public string Regulations { get; set; } = string.Empty;

    /// <summary>
    /// Notes specific to Priority Mail Express International service. Only used for international when ServiceType is US-GEM.
    /// </summary>
    public string GEMNotes { get; set; } = string.Empty;

    /// <summary>
    /// Restrictions on items being shipped based on country of destination. Only used for International.
    /// </summary>
    public string Restrictions { get; set; } = string.Empty;

    /// <summary>
    /// List of items prohibited from mailing based on country of destination. Only used for International.
    /// </summary>
    public string Prohibitions { get; set; } = string.Empty;

    /// <summary>
    /// Only returned for international mail classes,
    /// this field explains the maximum package dimensions for this mail class and destination country.
    /// </summary>
    public string MaxDimensions { get; set; } = string.Empty;
}
