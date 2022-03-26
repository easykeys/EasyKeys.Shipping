namespace EasyKeys.Shipping.Abstractions.Models
{
    public class Charges
    {
        /// <summary>
        /// Charge by the provider to customer.
        /// </summary>
        public decimal BaseCharge { get; set; }

        /// <summary>
        /// Actual charge to the shipper based on negotioated discount.
        /// </summary>
        public decimal NetCharge { get; set; }

        public Dictionary<string, decimal> SurCharges { get; set; } = new Dictionary<string, decimal>();

        public decimal TotalSurCharges { get; set; }
    }
}
