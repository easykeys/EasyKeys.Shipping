using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.API.Models
{
    public class InternationalShipmentDto : ShipmentDto
    {
        public Commodity? Commodity { get; set; }
    }
}
