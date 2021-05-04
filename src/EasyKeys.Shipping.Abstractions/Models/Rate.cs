using System;

namespace EasyKeys.Shipping.Abstractions
{
    public class Rate
    {
        public Rate(
            string name,
            string serviceName,
            decimal totalCharges,
            DateTime? guaranteedDelivery)
            : this(name, serviceName, totalCharges, 0.0M, guaranteedDelivery, false, "US")
        {
        }

        public Rate(
            string name,
            string serviceName,
            decimal totalCharges,
            decimal totalCharges2,
            DateTime? guaranteedDelivery) : this(name, serviceName, totalCharges, totalCharges2, guaranteedDelivery, false, "US")
        {
        }

        public Rate(
            string name,
            string serviceName,
            decimal totalCharges,
            DateTime? guaranteedDelivery,
            bool saturdayDelivery) : this(name, serviceName, totalCharges, 0.0M, guaranteedDelivery, saturdayDelivery, "US")
        {
        }

        public Rate(
            string name,
            string serviceName,
            decimal totalCharges,
            decimal totalCharges2,
            DateTime? guaranteedDelivery,
            bool saturdayDelivery,
            string currencyCode)
        {
            Name = name;
            ServiceName = serviceName;
            TotalCharges = totalCharges;
            TotalCharges2 = totalCharges2;
            GuaranteedDelivery = guaranteedDelivery;
            SaturdayDelivery = saturdayDelivery;
            CurrencyCode = currencyCode;
        }

        /// <summary>
        ///     A Name of the rate, as specified by the provider.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Service Provider Name.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        ///     The guaranteed date and time of delivery for this rate.
        /// </summary>
        public DateTime? GuaranteedDelivery { get; set; }

        /// <summary>
        /// Saturday Delivery indicator.
        /// </summary>
        public bool SaturdayDelivery { get; set; }

        /// <summary>
        ///     The total cost of this rate.
        /// </summary>
        public decimal TotalCharges { get; set; }

        /// <summary>
        /// In case with FedEx we can have two layers of charges.
        /// </summary>
        public decimal TotalCharges2 { get; set; }

        /// <summary>
        ///     Currency code, if applicable.
        /// </summary>
        public string CurrencyCode { get; }

        /// <summary>
        /// Provides with ability to pass the flag of free option, if the rules engine decides it can be free.
        /// </summary>
        public bool HasFreeOption { get; set; }
    }
}
