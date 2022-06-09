using EasyKeys.Shipping.Abstractions.Models;

namespace Minimal.Apis.Models;

public class FedExShipmentDto : StampsShipmentDto
{
    public Commodity? Commodity { get; set; }
}
