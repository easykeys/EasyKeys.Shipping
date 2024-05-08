using System.Text.Json.Serialization;

using EasyKeys.Shipping.FedEx.Abstractions.Models;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Response;

public class ResponseRoot
{
    [JsonPropertyName("transactionId")]
    public string? TransactionId { get; set; }

    [JsonPropertyName("customerTransactionId")]
    public string? CustomerTransactionId { get; set; }

    [JsonPropertyName("output")]
    public Output? Output { get; set; }

    [JsonPropertyName("errors")]
    public List<ApiError> Errors { get; set; } = new ();
}
