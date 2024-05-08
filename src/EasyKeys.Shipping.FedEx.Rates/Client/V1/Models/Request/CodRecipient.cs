using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class CodRecipient
{
    [JsonPropertyName("accountNumber")]
    public AccountNumber? AccountNumber { get; set; }
}
