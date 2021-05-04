using System.Collections.Generic;

namespace EasyKeys.Shipping.FedEx.AddressValidation
{
    public static class AddressAttributes
    {
#pragma warning disable SA1401 // Fields should be private
        public static Dictionary<string, string> SupportedCountries = new ()
#pragma warning restore SA1401 // Fields should be private
        {
            { "AR", "Argentina" },
            { "AW", "Aruba" },
            { "AU", "Australia" },
            { "AT", "Austria" },
            { "BS", "Bahamas" },
            { "BB", "Barbados" },
            { "BE", "Belgium" },
            { "BM", "Bermuda" },
            { "BR", "Brazil" },
            { "CA", "Canada" },
            { "KY", "Cayman Islands" },
            { "CL", "Chile" },
            { "CN", "China" },
            { "CR", "Costa Rica" },
            { "CZ", "Czech Republic" },
            { "DK", "Denmark" },
            { "DO", "Dominican Republic" },
            { "EE", "Estonia" },
            { "FI", "Finland" },
            { "DE", "Germany" },
            { "GR", "Greece" },
            { "GT", "Guatemala" },
            { "HK", "Hong Kong" },
            { "IT", "Italy" },
            { "JM", "Jamaica" },
            { "MY", "Malaysia" },
            { "MX", "Mexico" },
            { "NL", "Netherlands" },
            { "NZ", "New Zealand" },
            { "NO", "Norway" },
            { "PA", "Panama" },
            { "PE", "Peru" },
            { "PT", "Portugal" },
            { "SG", "Singapore" },
            { "ZA", "South Africa" },
            { "ES", "Spain" },
            { "SE", "Sweden" },
            { "CH", "Switzerland" },
            { "UK", "United Kingdom" },
            { "US", "United States" },
            { "UY", "Uruguay" },
            { "VE", "Venezuela" },
        };
    }
}
