using EasyKeys.Shipping.Abstractions.Models;

namespace Minimal.Apis.Models;

public class ShipmentDto
{
    public Address? Origin { get; set; }

    public Address? Destination { get; set; }

    public PackageDto? Package { get; set; }
}
