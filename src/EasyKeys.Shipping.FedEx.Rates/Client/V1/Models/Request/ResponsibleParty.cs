using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class ResponsibleParty
{
    [JsonPropertyName("address")]
    public Address? Address { get; set; }

    [JsonPropertyName("contact")]
    public Contact? Contact { get; set; }

    [JsonPropertyName("accountNumber")]
    public AccountNumber? AccountNumber { get; set; }
}
