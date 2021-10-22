namespace EasyKeys.Shipping.Usps.Rates;

public class UspsRateOptions
{
    public bool BaseRatesOnly { get; set; }

    public string ServiceName { get; set; } = string.Empty;

    public DateTime? DefaultGuaranteedDelivery { get; set; }
}
