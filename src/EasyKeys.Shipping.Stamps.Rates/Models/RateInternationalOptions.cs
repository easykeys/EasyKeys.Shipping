namespace EasyKeys.Shipping.Stamps.Rates.Models;

public class RateInternationalOptions : RateOptions
{
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
