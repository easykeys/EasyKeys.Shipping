using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.API.Models;

public class ShipmentDto
{
    public Address? Origin { get; set; }

    public Address? Destination { get; set; }

    public PackageDto? Package { get; set; }
}
