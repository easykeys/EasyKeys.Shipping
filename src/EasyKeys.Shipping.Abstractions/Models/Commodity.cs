namespace EasyKeys.Shipping.Abstractions.Models;

/// <summary>
/// Commodity information for International Shipments.
/// </summary>
public class Commodity
{
    /// <summary>
    /// Name of this commodity.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Required
    /// The total number of packages within the shipment
    /// that contains this commodity (can be >= PackageCount).
    /// </summary>
    public int NumberOfPieces { get; set; }

    /// <summary>
    /// Required
    /// A minimum of three characters is requred for this element. Maximum number of characters is 450.
    /// Make sure descriptions are specific to avoid any delays with customs.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Required
    /// Code of the country in which the commodity contents were produced or manufactured in their final form.
    /// </summary>
    public string CountryOfManufacturer { get; set; } = string.Empty;

    /// <summary>
    /// Optional
    /// For efficient customs clearance, a Harmonized Code should be included for all commodities.
    /// <seealso href="https://hts.usitc.gov/?query=8301"/>.
    /// </summary>
    public string HarmonizedCode { get; set; } = string.Empty;

    /// <summary>
    /// Required
    /// Total quantity of an individual commodity within this shipment (used in conjunction with QuantityUnits).
    /// For example, if your MPS contains 10 baseballs, 10 is included in this element as part of the
    /// commodity description of baseballs assuming that the QuantityUnits value is EA. Must be included for each
    /// commodity.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Optional
    /// An identifying mark or number used on the packaging of a shipment to help customs identify a particular shipment.
    /// </summary>
    public string CIMarksandNumbers { get; set; } = string.Empty;

    /// <summary>
    /// Required
    /// Unit of measure (for example : EA = each, DZ = dozen) for each commodity in the shipment.
    /// </summary>
    public string QuantityUnits { get; set; } = string.Empty;

    /// <summary>
    /// Required
    /// Customs value for each price of a particular commodity in the shipment.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Optional
    /// Total Customs Value for this line item. It should equal the commodity unit
    /// quantity x commodity unit value. Six explicit decimal positions. The maxium length
    /// is 18 including the decimal.
    /// </summary>
    public decimal CustomsValue { get; set; }

    /// <summary>
    /// Required
    /// At least one occurence is required for international commodity shipments.
    /// The Commodity/Amount must equal the commodity UnitPrrice x Units.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Required only if a commodity is shipped on a commercial export license.
    /// </summary>
    public string ExportLicenseNumber { get; set; } = string.Empty;

    /// <summary>
    /// Required only if a commodity is shipped on a commercial export license and the
    /// ExportLicenseNumber element is supplied.
    /// </summary>
    public DateTime ExportLicenseExpirationDate { get; set; }

    /// <summary>
    /// A maximum of four occurrences per commodity may be included.
    /// </summary>
    public string Comments { get; set; } = string.Empty;

    /// <summary>
    /// Optional
    /// Contains only additational quantitative information other than weight and quantity
    /// to calculate duties and taxes.
    /// </summary>
    public string AdditionalMeasures { get; set; } = string.Empty;

    /// <summary>
    /// Optional
    /// Defines additional characteristics of the commodity used to calculate duties and taxes.
    /// </summary>
    public string ExiseConditions { get; set; } = string.Empty;

    /// <summary>
    /// The part number of this commodity.
    /// </summary>
    public string PartNumber { get; set; } = string.Empty;

    /// <summary>
    /// The field is used for calcuation of duties and taxes.
    /// Valid values are BUSINESS and CONSUMER.
    /// </summary>
    public string Purpose { get; set; } = string.Empty;

    /// <summary>
    /// Weight of commodity in lbs.
    /// </summary>
    public decimal Weight { get; set; }
}
