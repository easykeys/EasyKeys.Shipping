using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class AlcoholDetail
{
    [JsonPropertyName("alcoholRecipientType")]
    public string? AlcoholRecipientType { get; set; }

    [JsonPropertyName("shipperAgreementType")]
    public string? ShipperAgreementType { get; set; }
}
