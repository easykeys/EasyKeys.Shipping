using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class Payor
{
    [JsonPropertyName("responsibleParty")]
    public ResponsibleParty? ResponsibleParty { get; set; }
}
