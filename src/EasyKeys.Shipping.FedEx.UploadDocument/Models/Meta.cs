using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.UploadDocument.Models;

public class Meta
{
    [JsonPropertyName("imageType")]
    public string ImageType { get; set; }

    [JsonPropertyName("imageIndex")]
    public string ImageIndex { get; set; }
}
