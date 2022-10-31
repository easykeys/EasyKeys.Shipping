namespace EasyKeys.Shipping.FedEx.Abstractions.Options;

public class FedExOptions
{
    public string Url => IsDevelopment ? "https://wsbeta.fedex.com:443/web-services" : "https://ws.fedex.com:443/web-services";

    public string FedExKey { get; set; } = string.Empty;

    public string FedExPassword { get; set; } = string.Empty;

    public string FedExAccountNumber { get; set; } = string.Empty;

    public string FedExMeterNumber { get; set; } = string.Empty;

    public bool IsDevelopment { get; set; }
}
