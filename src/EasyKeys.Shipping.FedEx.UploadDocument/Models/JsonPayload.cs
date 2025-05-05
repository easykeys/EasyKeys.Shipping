using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EasyKeys.Shipping.FedEx.UploadDocument.Models;
public class JsonPayload
{
    [JsonPropertyName("document")]
    public Document Document { get; set; }

    [JsonPropertyName("rules")]
    public Rules Rules { get; set; }

    public string FieldName { get; set; } = "document"; // form field name
}
