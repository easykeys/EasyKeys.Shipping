using EasyKeys.Shipping.Abstractions.Models;

namespace Minimal.Apis.Models;

public class AddressValidationDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public Address Address { get; set; } = new Address();
}
