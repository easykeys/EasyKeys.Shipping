using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class CustomsClearanceDetail
{
    [JsonPropertyName("brokers")]
    public List<Broker>? Brokers { get; set; }

    [JsonPropertyName("commercialInvoice")]
    public CommercialInvoice? CommercialInvoice { get; set; }

    [JsonPropertyName("freightOnValue")]
    public string? FreightOnValue { get; set; }

    [JsonPropertyName("dutiesPayment")]
    public DutiesPayment? DutiesPayment { get; set; }

    [JsonPropertyName("commodities")]
    public List<Commodity>? Commodities { get; set; }
}
