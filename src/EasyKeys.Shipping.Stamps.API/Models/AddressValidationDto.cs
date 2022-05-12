using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.API.Models;

public class AddressValidationDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public Address? Address { get; set; }
}
