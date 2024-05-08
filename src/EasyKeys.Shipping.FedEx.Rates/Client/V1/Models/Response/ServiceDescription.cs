using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Response;

public class ServiceDescription
{
    [JsonPropertyName("serviceId")]
    public string? ServiceId { get; set; }

    [JsonPropertyName("serviceType")]
    public string? ServiceType { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("names")]
    public List<Name>? Names { get; set; }

    [JsonPropertyName("operatingOrgCodes")]
    public List<string>? OperatingOrgCodes { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("astraDescription")]
    public string? AstraDescription { get; set; }

    [JsonPropertyName("serviceCategory")]
    public string? ServiceCategory { get; set; }
}
