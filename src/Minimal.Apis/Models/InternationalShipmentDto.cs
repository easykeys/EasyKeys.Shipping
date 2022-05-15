using EasyKeys.Shipping.Abstractions.Models;

namespace Minimal.Apis.Models;

public class InternationalShipmentDto : ShipmentDto
{
    public Commodity? Commodity { get; set; }
}
