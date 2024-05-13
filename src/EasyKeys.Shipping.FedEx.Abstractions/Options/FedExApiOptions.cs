using Newtonsoft.Json;

namespace EasyKeys.Shipping.FedEx.Abstractions.Options;

public class FedExApiOptions
{
    public string Url => IsDevelopment ? "https://apis-sandbox.fedex.com/" : "https://apis.fedex.com/";

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string FedExAccountNumber { get; set; } = string.Empty;

    public bool IsDevelopment { get; set; }

    public JsonSerializerSettings JsonSerializerOptions { get; set; } = new() { DefaultValueHandling = DefaultValueHandling.Ignore };
}
