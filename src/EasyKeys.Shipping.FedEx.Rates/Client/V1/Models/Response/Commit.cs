using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Response;

public class Commit
{
    [JsonPropertyName("dateDetail")]
    public DateDetail? DateDetail { get; set; }
}
