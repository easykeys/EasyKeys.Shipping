using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class ServiceTypeDetail
{
    [JsonPropertyName("carrierCode")]
    public string? CarrierCode { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("serviceName")]
    public string? ServiceName { get; set; }

    [JsonPropertyName("serviceCategory")]
    public string? ServiceCategory { get; set; }
}
