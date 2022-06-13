using EasyKeys.Shipping.Abstractions.Models;

namespace Minimal.Apis.Models;

public class InternationalShipmentDto : StampsShipmentDto
{
    public Commodity? Commodity { get; set; }
}
