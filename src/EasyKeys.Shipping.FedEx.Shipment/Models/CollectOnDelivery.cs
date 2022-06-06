﻿namespace EasyKeys.Shipping.FedEx.Shipment.Models;

public class CollectOnDelivery
{
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
