using EasyKeys.Shipping.PostalAddress;

using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.UnitTest;

public class AddressParserTests
{
    private readonly IAddressParser _parser;

    public AddressParserTests()
    {
        var services = new ServiceCollection();

        services.AddAddressParser();

        var sp = services.BuildServiceProvider();

        _parser = sp.GetRequiredService<IAddressParser>();
    }

    [Fact]
    public void CanParseCanadianAddress()
    {
        var result = _parser.TryParseAddress("32 Garryoaks Drive, Brampton, ON L6P 3E3", out var address);
        Assert.False(result);
    }

    [Fact]
    public void CanParseSuiteAndFloor()
    {
        var result = _parser.TryParseAddress("2929 WALNUT ST Floor 10 Suite 1000 PHILADELPHIA PA 19104-5054", out var address);
        Assert.True(result);
    }

    [Fact]
    public void CanParseTypicalAddressWithoutPunctuationAfterStreetLine()
    {
        var result = _parser.TryParseAddress("1005 N Gravenstein Highway Sebastopol, CA 95472", out var address);

        Assert.True(result);

        Assert.Equal("SEBASTOPOL", address?.City);
        Assert.Equal("1005", address?.Number);
        Assert.Null(address?.Postdirectional);
        Assert.Equal("N", address?.Predirectional);
        Assert.Null(address?.SecondaryNumber);
        Assert.Null(address?.SecondaryUnit);
        Assert.Equal("CA", address?.State);
        Assert.Equal("GRAVENSTEIN", address?.Street);
        Assert.Equal("1005 N GRAVENSTEIN HWY", address?.StreetLine);
        Assert.Equal("HWY", address?.Suffix);
        Assert.Equal("95472", address?.Zip);
    }

    [Fact]
    public void CanParseTypicalAddressWithPunctuation()
    {
        var result = _parser.TryParseAddress("1005 N Gravenstein Highway, Sebastopol, CA 95472", out var address);

        Assert.True(result);

        Assert.Equal("SEBASTOPOL", address?.City);
        Assert.Equal("1005", address?.Number);
        Assert.Null(address?.Postdirectional);
        Assert.Equal("N", address?.Predirectional);
        Assert.Null(address?.SecondaryNumber);
        Assert.Null(address?.SecondaryUnit);
        Assert.Equal("CA", address?.State);
        Assert.Equal("GRAVENSTEIN", address?.Street);
        Assert.Equal("1005 N GRAVENSTEIN HWY", address?.StreetLine);
        Assert.Equal("HWY", address?.Suffix);
        Assert.Equal("95472", address?.Zip);
    }

    [Fact]
    public void CanParseAddressWithRangelessSecondaryUnit()
    {
        var result = _parser.TryParseAddress("1050 Broadway Penthouse, New York, NY 10001", out var address);

        Assert.True(result);

        Assert.Equal("NEW YORK", address?.City);
        Assert.Equal("1050", address?.Number);
        Assert.Null(address?.Postdirectional);
        Assert.Null(address?.Predirectional);
        Assert.Null(address?.SecondaryNumber);
        Assert.Equal("PH", address?.SecondaryUnit);
        Assert.Equal("NY", address?.State);
        Assert.Equal("BROADWAY", address?.Street);
        Assert.Equal("1050 BROADWAY PH", address?.StreetLine);
        Assert.Null(address?.Suffix);
        Assert.Equal("10001", address?.Zip);
    }

    [Fact]
    public void CanParsePostOfficeBoxAddress()
    {
        var result = _parser.TryParseAddress("P.O. BOX 4857, New York, NY 10001", out var address);

        Assert.True(result);

        Assert.Equal("NEW YORK", address?.City);
        Assert.Null(address?.Number);
        Assert.Null(address?.Postdirectional);
        Assert.Null(address?.Predirectional);
        Assert.Null(address?.SecondaryNumber);
        Assert.Null(address?.SecondaryUnit);
        Assert.Equal("NY", address?.State);
        Assert.Null(address?.Street);
        Assert.Equal("PO BOX 4857", address?.StreetLine);
        Assert.Null(address?.Suffix);
        Assert.Equal("10001", address?.Zip);
    }

    /// <summary>
    /// Military addresses seem to follow no convention whatsoever in the
    /// street line, but the APO/FPO/DPO AA/AE/AP 9NNNN part of the place line
    /// is pretty well standardized. I've made a special exception for these
    /// kinds of addresses so that the street line is just dumped as-is into
    /// the StreetLine field.
    /// </summary>
    [Fact]
    public void CanParseMilitaryAddress()
    {
        var result = _parser.TryParseAddress("PSC BOX 453, APO AE 99969", out var address);

        Assert.True(result);

        Assert.Equal("APO", address?.City);
        Assert.Null(address?.Number);
        Assert.Null(address?.Postdirectional);
        Assert.Null(address?.Predirectional);
        Assert.Null(address?.SecondaryNumber);
        Assert.Null(address?.SecondaryUnit);
        Assert.Equal("AE", address?.State);
        Assert.Null(address?.Street);
        Assert.Equal("PSC BOX 453", address?.StreetLine);
        Assert.Null(address?.Suffix);
        Assert.Equal("99969", address?.Zip);
    }

    [Fact]
    public void CanParseAddressWithoutPunctuation()
    {
        var result = _parser.TryParseAddress("999 West 89th Street Apt A New York NY 10024", out var address);

        Assert.True(result);

        Assert.Equal("NEW YORK", address?.City);
        Assert.Equal("999", address?.Number);
        Assert.Null(address?.Postdirectional);
        Assert.Equal("W", address?.Predirectional);
        Assert.Equal("A", address?.SecondaryNumber);
        Assert.Equal("APT", address?.SecondaryUnit);
        Assert.Equal("NY", address?.State);
        Assert.Equal("89TH", address?.Street);
        Assert.Equal("999 W 89TH ST APT A", address?.StreetLine);
        Assert.Equal("ST", address?.Suffix);
        Assert.Equal("10024", address?.Zip);
    }

    /// <summary>
    /// Grid-style addresses are common in parts of Utah. The official USPS address database
    /// in this case treats "E" as a predirectional, "1700" as the street name, and "S" as a
    /// postdirectional, and nothing as the suffix, so that's how we parse it, too.
    /// </summary>
    [Fact]
    public void CanParseGridStyleAddress()
    {
        var result = _parser.TryParseAddress("842 E 1700 S, Salt Lake City, UT 84105", out var address);

        Assert.True(result);

        Assert.Equal("SALT LAKE CITY", address?.City);
        Assert.Equal("842", address?.Number);
        Assert.Equal("S", address?.Postdirectional);
        Assert.Equal("E", address?.Predirectional);
        Assert.Null(address?.SecondaryNumber);
        Assert.Null(address?.SecondaryUnit);
        Assert.Equal("UT", address?.State);
        Assert.Equal("1700", address?.Street);
        Assert.Equal("842 E 1700 S", address?.StreetLine);
        Assert.Null(address?.Suffix);
        Assert.Equal("84105", address?.Zip);
    }

    /// <summary>
    /// People in Wisconsin and Illinois are eating too much cheese, apparently, because
    /// you can encounter house numbers with letters in them. It's similar to the
    /// Utah grid-system, except the gridness is all crammed into the house number.
    /// </summary>
    [Fact]
    public void CanParseAddressWithAlphanumericRange()
    {
        var result = _parser.TryParseAddress("N6W23001 BLUEMOUND ROAD, ROLLING MEADOWS, IL, 12345", out var address);

        Assert.True(result);

        Assert.Equal("ROLLING MEADOWS", address?.City);
        Assert.Equal("N6W23001", address?.Number);
        Assert.Null(address?.Postdirectional);
        Assert.Null(address?.Predirectional);
        Assert.Null(address?.SecondaryNumber);
        Assert.Null(address?.SecondaryUnit);
        Assert.Equal("IL", address?.State);
        Assert.Equal("BLUEMOUND", address?.Street);
        Assert.Equal("N6W23001 BLUEMOUND RD", address?.StreetLine);
        Assert.Equal("RD", address?.Suffix);
        Assert.Equal("12345", address?.Zip);
    }

    /// <summary>
    /// Speaking of weird addresses, sometimes people put a space in the number.
    /// USPS says we should squash it together.
    /// </summary>
    [Fact]
    public void CanParseAddressWithSpacedAlphanumericRange()
    {
        var result = _parser.TryParseAddress("N645 W23001 BLUEMOUND ROAD, ROLLING MEADOWS, IL, 12345", out var address);

        Assert.True(result);

        Assert.Equal("ROLLING MEADOWS", address?.City);
        Assert.Equal("N645W23001", address?.Number);
        Assert.Null(address?.Postdirectional);
        Assert.Null(address?.Predirectional);
        Assert.Null(address?.SecondaryNumber);
        Assert.Null(address?.SecondaryUnit);
        Assert.Equal("IL", address?.State);
        Assert.Equal("BLUEMOUND", address?.Street);
        Assert.Equal("N645W23001 BLUEMOUND RD", address?.StreetLine);
        Assert.Equal("RD", address?.Suffix);
        Assert.Equal("12345", address?.Zip);
    }

    /// <summary>
    /// In parts of New York City, some people feel REALLY STRONGLY about
    /// the hyphen in their house number. The numbering system makes sense,
    /// but the USPS address database doesn't support hyphens in the number field.
    /// To the USPS, the hyphen does not exist, but the DMM specifically does say
    /// that "if present, the hyphen should not be removed.".
    /// </summary>
    [Fact]
    public void CanParseQueensStyleAddress()
    {
        var result = _parser.TryParseAddress("123-465 34th St New York NY 12345", out var address);

        Assert.True(result);

        Assert.Equal("NEW YORK", address?.City);
        Assert.Equal("123-465", address?.Number);
        Assert.Null(address?.Postdirectional);
        Assert.Null(address?.Predirectional);
        Assert.Null(address?.SecondaryNumber);
        Assert.Null(address?.SecondaryUnit);
        Assert.Equal("NY", address?.State);
        Assert.Equal("34TH", address?.Street);
        Assert.Equal("123-465 34TH ST", address?.StreetLine);
        Assert.Equal("ST", address?.Suffix);
        Assert.Equal("12345", address?.Zip);
    }

    /// <summary>
    /// In Virginia Beach, for example, there's a South Blvd, which could really
    /// throw a spanner into our predirectional/postdirectional parsing. We call
    /// this case out specifically in our regex.
    /// </summary>
    [Fact]
    public void CanParseAddressWithCardinalStreetName()
    {
        var result = _parser.TryParseAddress("500 SOUTH STREET VIRGINIA BEACH VIRGINIA 23452", out var address);

        Assert.True(result);

        Assert.Equal("VIRGINIA BEACH", address?.City);
        Assert.Equal("500", address?.Number);
        Assert.Null(address?.Postdirectional);
        Assert.Null(address?.Predirectional);
        Assert.Null(address?.SecondaryNumber);
        Assert.Null(address?.SecondaryUnit);
        Assert.Equal("VA", address?.State);
        Assert.Equal("SOUTH", address?.Street);
        Assert.Equal("500 SOUTH ST", address?.StreetLine);
        Assert.Equal("ST", address?.Suffix);
        Assert.Equal("23452", address?.Zip);
    }

    /// <summary>
    /// When people live in apartments with letters, they sometimes attach the apartment
    /// letter to the end of the house number. This is wrong, and these people need to be
    /// lined up and individually slapped. We pull out the unit and designate it as "APT",
    /// which in my experience is the designator that USPS uses in the vast, vast majority
    /// of cases.
    /// </summary>
    [Fact]
    public void CanParseAddressWithRangedUnitAttachedToNumber()
    {
        var result = _parser.TryParseAddress("403D BERRYFIELD LANE CHESAPEAKE VA 23224", out var address);

        Assert.True(result);

        Assert.Equal("CHESAPEAKE", address?.City);
        Assert.Equal("403", address?.Number);
        Assert.Null(address?.Postdirectional);
        Assert.Null(address?.Predirectional);
        Assert.Equal("D", address?.SecondaryNumber);
        Assert.Equal("APT", address?.SecondaryUnit);
        Assert.Equal("VA", address?.State);
        Assert.Equal("BERRYFIELD", address?.Street);
        Assert.Equal("403 BERRYFIELD LN APT D", address?.StreetLine);
        Assert.Equal("LN", address?.Suffix);
        Assert.Equal("23224", address?.Zip);
    }

    /// <summary>
    /// At least it's not platform 9 3/4.
    /// </summary>
    [Fact]
    public void CanParseFractionalAddress()
    {
        var result = _parser.TryParseAddress("123 1/2 MAIN ST, RICHMOND, VA 23221", out var address);

        Assert.True(result);

        Assert.Equal("RICHMOND", address?.City);
        Assert.Equal("123 1/2", address?.Number);
        Assert.Null(address?.Postdirectional);
        Assert.Null(address?.Predirectional);
        Assert.Null(address?.SecondaryNumber);
        Assert.Null(address?.SecondaryUnit);
        Assert.Equal("VA", address?.State);
        Assert.Equal("MAIN", address?.Street);
        Assert.Equal("123 1/2 MAIN ST", address?.StreetLine);
        Assert.Equal("ST", address?.Suffix);
        Assert.Equal("23221", address?.Zip);
    }
}
