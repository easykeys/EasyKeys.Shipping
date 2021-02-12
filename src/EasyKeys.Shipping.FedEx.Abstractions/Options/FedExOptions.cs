namespace EasyKeys.Shipping.FedEx.Abstractions.Options
{
    public class FedExOptions
    {
        public string Url { get; set; } = "https://ws.fedex.com:443/web-services";

        public string FedExKey { get; set; } = string.Empty;

        public string FedExPassword { get; set; } = string.Empty;

        public string FedExAccountNumber { get; set; } = string.Empty;

        public string FedExMeterNumber { get; set; } = string.Empty;
    }
}
