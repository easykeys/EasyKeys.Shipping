namespace EasyKeys.Shipping.FedEx.Shipment.Models;

public class FedExCollectOnDelivery
{
    /// <summary>
    /// guaranteed_funds, cash, company_check, personal_check.
    /// </summary>
    public FedExCollectionType CollectionType { get; set; } = FedExCollectionType.GuaranteedFunds;

    public decimal Amount { get; set; }

    /// <summary>
    /// default to USD.
    /// </summary>
    public string Currency { get; set; } = "USD";
}
