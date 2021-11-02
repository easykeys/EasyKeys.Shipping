namespace EasyKeys.Shipping.Usps.Abstractions.Options;

public class UspsOptions
{
    public string UserId { get; set; } = string.Empty;

    public Uri BaseUri { get; set; } = new Uri("https://secure.shippingapis.com/ShippingAPI.dll");

    public string ClientIp { get; set; } = string.Empty;

    public string? SourceId { get; set; }
}
