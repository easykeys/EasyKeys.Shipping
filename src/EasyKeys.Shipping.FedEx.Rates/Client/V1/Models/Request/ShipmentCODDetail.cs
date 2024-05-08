using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class ShipmentCODDetail
{
    [JsonPropertyName("addTransportationChargesDetail")]
    public AddTransportationChargesDetail? AddTransportationChargesDetail { get; set; }

    [JsonPropertyName("codRecipient")]
    public CodRecipient? CodRecipient { get; set; }

    [JsonPropertyName("remitToName")]
    public string? RemitToName { get; set; }

    [JsonPropertyName("codCollectionType")]
    public string? CodCollectionType { get; set; }

    [JsonPropertyName("financialInstitutionContactAndAddress")]
    public FinancialInstitutionContactAndAddress? FinancialInstitutionContactAndAddress { get; set; }

    [JsonPropertyName("returnReferenceIndicatorType")]
    public string? ReturnReferenceIndicatorType { get; set; }
}
