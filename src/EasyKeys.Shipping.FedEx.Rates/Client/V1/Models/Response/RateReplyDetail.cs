using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Response;

public class RateReplyDetail
{
    [JsonPropertyName("serviceType")]
    public string ServiceType { get; set; } = string.Empty;

    [JsonPropertyName("serviceName")]
    public string ServiceName { get; set; } = string.Empty;

    [JsonPropertyName("packagingType")]
    public string PackagingType { get; set; } = string.Empty;

    [JsonPropertyName("customerMessages")]
    public List<CustomerMessage>? CustomerMessages { get; set; }

    [JsonPropertyName("ratedShipmentDetails")]
    public List<RatedShipmentDetail> RatedShipmentDetails { get; set; } = new ();

    [JsonPropertyName("anonymouslyAllowable")]
    public bool? AnonymouslyAllowable { get; set; }

    [JsonPropertyName("operationalDetail")]
    public OperationalDetail OperationalDetail { get; set; } = new ();

    [JsonPropertyName("signatureOptionType")]
    public string? SignatureOptionType { get; set; }

    [JsonPropertyName("serviceDescription")]
    public ServiceDescription? ServiceDescription { get; set; }

    [JsonPropertyName("commit")]
    public Commit Commit { get; set; } = new ();
}
