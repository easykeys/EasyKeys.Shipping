using System;

namespace EasyKeys.Shipping.Usps.Rates
{
    public class UspsRateOptions
    {
        public bool BaseRatesOnly { get; set; } = false;

        public string ServiceName { get; set; }

        public DateTime? DefaultGuaranteedDelivery { get; set; }
    }
}
