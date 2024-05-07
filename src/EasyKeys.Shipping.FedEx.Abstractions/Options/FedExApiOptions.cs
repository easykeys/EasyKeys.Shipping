namespace EasyKeys.Shipping.FedEx.Abstractions.Options;

public class FedExApiOptions
{
    public string Url => IsDevelopment ? "https://apis-sandbox.fedex.com/" : "https://apis-sandbox.fedex.com/";

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;
  
    public bool IsDevelopment { get; set; }
}
