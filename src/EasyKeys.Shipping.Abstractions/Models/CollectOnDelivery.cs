namespace EasyKeys.Shipping.Abstractions.Models;

public class CollectOnDelivery
{
    public bool Activated { get; set; }

    /// <summary>
    /// guaranteed_funds, cash, company_check, personal_check.
    /// </summary>
    public string CollectionType { get; set; } = "GUARANTEED_FUNDS";

    public decimal Amount { get; set; }

    /// <summary>
    /// default to USD.
    /// </summary>
    public string Currency { get; set; } = "USD";
}
