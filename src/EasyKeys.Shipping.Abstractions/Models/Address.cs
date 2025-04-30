using System.Globalization;
using System.Text;

namespace EasyKeys.Shipping.Abstractions.Models;

public class Address : ValueObject
{
    public Address()
    {
    }

    public Address(
        string city,
        string stateOrProvince,
        string postalCode,
        string countryCode)
        : this(string.Empty, string.Empty, city, stateOrProvince, postalCode, countryCode)
    {
    }

    public Address(
        string streetLine,
        string city,
        string stateOrProvince,
        string postalCode,
        string countryCode)
        : this(streetLine, string.Empty, city, stateOrProvince, postalCode, countryCode)
    {
    }

    public Address(
        string streetLine,
        string city,
        string stateOrProvince,
        string postalCode,
        string countryCode,
        bool isResidential)
    : this(streetLine, string.Empty, city, stateOrProvince, postalCode, countryCode, isResidential)
    {
    }

    public Address(
        string streetLine,
        string streetLine1,
        string city,
        string stateOrProvince,
        string postalCode,
        string countryCode,
        bool isResidential = false)
    {
        StreetLine = streetLine;
        StreetLine2 = streetLine1;
        City = city;
        StateOrProvince = stateOrProvince;
        PostalCode = postalCode;
        CountryCode = countryCode;
        IsResidential = isResidential;
    }

    public string City { get; set; } = string.Empty;

    public string CountryCode { get; set; } = string.Empty;

    public string CountryName { get; set; } = string.Empty;

    public string StreetLine { get; set; } = string.Empty;

    public string StreetLine2 { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string StateOrProvince { get; set; } = string.Empty;

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

        try
        {
            var regionInfo = new RegionInfo(GetCountryCode());
            return regionInfo.EnglishName;
        }
        catch
        {
            // causes the whole application to crash.
        }

        return string.Empty;
    }

    public string GetCountryCode()
    {
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

        return countryCode;
    }

    public bool IsCanadaAddress()
    {
        return !string.IsNullOrEmpty(CountryCode) && string.Equals(CountryCode, "CA", StringComparison.OrdinalIgnoreCase);
    }

    public bool IsMexicoAddress()
    {
        return !string.IsNullOrEmpty(CountryCode) && string.Equals(CountryCode, "MX", StringComparison.OrdinalIgnoreCase);
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

    public bool IsUnitedStatesTerritory()
    {
        var usAndTerritories = new List<string> { "AS", "GU", "MP", "PR", "UM", "VI" };

        return usAndTerritories.Contains(StateOrProvince) || usAndTerritories.Contains(CountryCode);
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendFormat(
            "{0} {1}, {2}, {3}, {4}, {5}",
            StreetLine,
            StreetLine2,
            City,
            StateOrProvince,
            PostalCode,
            CountryCode);
        return builder.ToString();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return City;
        yield return CountryCode;
        yield return CountryName;
        yield return StreetLine;
        yield return StreetLine2;
        yield return PostalCode;
        yield return StateOrProvince;
        yield return IsResidential;
    }
}
