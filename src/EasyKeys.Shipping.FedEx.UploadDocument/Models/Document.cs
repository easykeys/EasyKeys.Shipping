using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.UploadDocument.Models;

public class Document
{
    [JsonPropertyName("referenceId")]
    public string ReferenceId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("contentType")]
    public string ContentType { get; set; }

    [JsonPropertyName("meta")]
    public Meta Meta { get; set; }
}
