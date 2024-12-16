namespace EasyKeys.Shipping.Amazon.Abstractions.Options;

public class AmazonShippingApiOptions
{
    public bool IsDevelopment { get; set; } = true;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;
}
