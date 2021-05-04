using System;
using System.Collections.Generic;
using System.Linq;

using EasyKeys.Shipping.Abstractions;

namespace EasyKeys.Shipping.FedEx.Extensions
{
    public static class RateAddressExtensions
    {
        /// <summary>
        /// Get FedEx API address from ShippingRates.Address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static RateClient.v28.Address GetFedExAddress(this Address address)
        {
            address = address ?? throw new ArgumentNullException(nameof(address));

            return new RateClient.v28.Address
            {
                StreetLines = GetStreetLines(address),
                City = address.City?.Trim(),
                StateOrProvinceCode = address.State?.Trim(),
                PostalCode = address.PostalCode?.Trim(),
                CountryCode = address.CountryCode?.Trim(),
                CountryName = address.GetCountryName(),
                Residential = address.IsResidential,
                ResidentialSpecified = address.IsResidential,
            };
        }

        /// <summary>
        /// Get street lines array.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private static string[] GetStreetLines(Address address)
        {
            var streetLines = new List<string>
            {
                address.Line1.Trim(),
                address.Line2.Trim(),
                address.Line3.Trim()
            };
            streetLines = streetLines.Where(l => !string.IsNullOrEmpty(l)).ToList();
            return streetLines.Any() ? streetLines.ToArray() : new string[] { string.Empty };
        }
    }
}
