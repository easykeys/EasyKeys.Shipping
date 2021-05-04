using System;
using System.Collections.Generic;
using System.Globalization;

namespace EasyKeys.Shipping.Abstractions
{
    public class Address
    {
        public Address()
        {
        }

        public Address(
            string city,
            string state,
            string postalCode,
            string countryCode)
            : this(string.Empty, string.Empty, string.Empty, city, state, postalCode, countryCode)
        {
        }

        public Address(
            string line1,
            string city,
            string state,
            string postalCode,
            string countryCode)
            : this(line1, string.Empty, string.Empty, city, state, postalCode, countryCode)
        {
        }

        public Address(
            string line1,
            string city,
            string state,
            string postalCode,
            string countryCode,
            bool isResidential)
        : this(line1, string.Empty, string.Empty, city, state, postalCode, countryCode, isResidential)
        {
        }

        public Address(
            string line1,
            string line2,
            string line3,
            string city,
            string state,
            string postalCode,
            string countryCode,
            bool isResidential = false)
        {
            Line1 = line1;
            Line2 = line2;
            Line3 = line3;
            City = city;
            State = state;
            PostalCode = postalCode;
            CountryCode = countryCode;
            IsResidential = isResidential;
        }

        public string City { get; set; } = string.Empty;

        public string CountryCode { get; set; } = string.Empty;

        public string CountryName { get; set; } = string.Empty;

        public string Line1 { get; set; } = string.Empty;

        public string Line2 { get; set; } = string.Empty;

        public string Line3 { get; set; } = string.Empty;

        public string PostalCode { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public bool IsResidential { get; set; }

        public string GetCountryName()
        {
            if (!string.IsNullOrEmpty(CountryName))
            {
                return CountryName;
            }

            if (string.IsNullOrEmpty(CountryCode))
            {
                return string.Empty;
            }

            var countryCode = CountryCode;

            // UK = GB United Kingdom
            // EI = IE Ireland
            // FX = FR France, Metropolitan
            if (string.Equals(countryCode, "UK", StringComparison.OrdinalIgnoreCase))
            {
                countryCode = "GB";
            }

            if (string.Equals(countryCode, "EI", StringComparison.OrdinalIgnoreCase))
            {
                countryCode = "IE";
            }

            if (string.Equals(countryCode, "FX", StringComparison.OrdinalIgnoreCase))
            {
                countryCode = "FR";
            }

            try
            {
                var regionInfo = new RegionInfo(countryCode);
                return regionInfo.EnglishName;
            }
            catch
            {
                // causes the whole application to crash.
            }

            return string.Empty;
        }

        public bool IsCanadaAddress()
        {
            return !string.IsNullOrEmpty(CountryCode) && string.Equals(CountryCode, "CA", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Returns true if the CountryCode matches US or one of the US territories.
        /// </summary>
        /// <returns></returns>
        public bool IsUnitedStatesAddress()
        {
            var usAndTerritories = new List<string> { "AS", "GU", "MP", "PR", "UM", "VI", "US" };

            return usAndTerritories.Contains(CountryCode);
        }
    }
}
