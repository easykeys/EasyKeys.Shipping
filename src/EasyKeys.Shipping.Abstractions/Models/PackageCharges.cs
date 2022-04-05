namespace EasyKeys.Shipping.Abstractions.Models;

public class PackageCharges
{
    /// <summary>
    /// Charge by the provider to customer.
    /// </summary>
    public decimal BaseCharge { get; set; }

    /// <summary>
    /// Actual charge to the shipper based on negotioated discount.
    /// </summary>
    public decimal NetCharge { get; set; }

    /// <summary>
    /// Surchage for the <see cref="Package"/>.
    /// </summary>
    public Dictionary<string, decimal> Surcharges { get; set; } = new Dictionary<string, decimal>();

    /// <summary>
    /// Total charge for <see cref="Package"/>.
    /// </summary>
    public decimal TotalSurCharges { get; set; }
}
