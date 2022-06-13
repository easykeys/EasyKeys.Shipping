using EasyKeys.Shipping.Abstractions.Models;

namespace Minimal.Apis.Models;

public class ShipmentDto
{
    public Address Origin { get; set; } = new Address
    {
        StreetLine = "11407 Granite Street",
        City = "Charlotte",
        StateOrProvince = "NC",
        PostalCode = "28273",
        CountryCode = "US"
    };

    public Address? Destination { get; set; }

    public PackageDto? Package { get; set; }
}
