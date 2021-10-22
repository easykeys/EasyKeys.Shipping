using AddressValidationClient.v4;

namespace EasyKeys.Shipping.FedEx.AddressValidation;

public static class AddressAttributes
{
#pragma warning disable SA1401 // Fields should be private
    public static Dictionary<string, string> SupportedCountries = new ()
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

    public static Dictionary<string, Flag> Flags = new ()
    {
        { "CountrySupported", new Flag(typeof(bool), new[] { OperationalAddressStateType.NORMALIZED, OperationalAddressStateType.STANDARDIZED }) },
        { "ZIP11Match", new Flag(typeof(bool), new[] { OperationalAddressStateType.NORMALIZED, OperationalAddressStateType.STANDARDIZED }) },
        { "SuiteRequiredButMissing", new Flag(typeof(bool), new[] { OperationalAddressStateType.NORMALIZED, OperationalAddressStateType.STANDARDIZED }) },
        { "InvalidSuiteNumber", new Flag(typeof(bool), new[] { OperationalAddressStateType.NORMALIZED, OperationalAddressStateType.STANDARDIZED }) },
        { "MultipleMatches", new Flag(typeof(bool), new[] { OperationalAddressStateType.NORMALIZED, OperationalAddressStateType.STANDARDIZED }) },
        { "Resolved", new Flag(typeof(bool), new[] { OperationalAddressStateType.NORMALIZED, OperationalAddressStateType.STANDARDIZED }) },
        { "ZIP4Match", new Flag(typeof(bool), new[] { OperationalAddressStateType.NORMALIZED, OperationalAddressStateType.STANDARDIZED }) },
        { "DPV", new Flag(typeof(bool), new[] { OperationalAddressStateType.NORMALIZED, OperationalAddressStateType.STANDARDIZED }) },

        // standardized
        { "ValidMultiUnit", new Flag(typeof(bool), new[] { OperationalAddressStateType.STANDARDIZED }) },
        { "POBox", new Flag(typeof(bool), new[] { OperationalAddressStateType.STANDARDIZED }) },
        { "MultiUnitBase", new Flag(typeof(bool), new[] { OperationalAddressStateType.STANDARDIZED }) },
        { "StreetAddress", new Flag(typeof(bool), new[] { OperationalAddressStateType.STANDARDIZED }) },
        { "POBoxOnlyZIP", new Flag(typeof(bool), new[] { OperationalAddressStateType.STANDARDIZED }) },
        { "UniqueZIP", new Flag(typeof(bool), new[] { OperationalAddressStateType.STANDARDIZED }) },
        { "SplitZIP", new Flag(typeof(bool), new[] { OperationalAddressStateType.STANDARDIZED }) },
        { "RRConversion", new Flag(typeof(bool), new[] { OperationalAddressStateType.STANDARDIZED }) },

        // normalized
        { "PostalValidated", new Flag(typeof(bool), new[] { OperationalAddressStateType.NORMALIZED }) },
        { "GeneralDelivery", new Flag(typeof(bool), new[] { OperationalAddressStateType.NORMALIZED }) },
        { "StreetRangeValidated", new Flag(typeof(bool), new[] { OperationalAddressStateType.NORMALIZED }) },
        { "MissingOrAmbiguousDirectional", new Flag(typeof(bool), new[] { OperationalAddressStateType.NORMALIZED }) },
        { "CityStateValidated", new Flag(typeof(bool), new[] { OperationalAddressStateType.NORMALIZED }) },

        // all
        { "Classification", new Flag(typeof(string), new[] { OperationalAddressStateType.NORMALIZED, OperationalAddressStateType.STANDARDIZED, OperationalAddressStateType.RAW }) },
        { "State", new Flag(typeof(string), new[] { OperationalAddressStateType.NORMALIZED, OperationalAddressStateType.STANDARDIZED, OperationalAddressStateType.RAW }) },
    };
#pragma warning restore SA1401 // Fields should be private
}
