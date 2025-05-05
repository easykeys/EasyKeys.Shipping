using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.UploadDocument.Models;

public class Rules
{
    [JsonPropertyName("workflowName")]
    public string WorkflowName { get; set; }
}
