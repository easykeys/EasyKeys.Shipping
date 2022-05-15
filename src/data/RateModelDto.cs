using EasyKeys.Shipping.Abstractions.Models;

namespace Models;

public class RateModelDto
{
    public Address Address { get; set; } = new Address();

    public IList<Package> Packages { get; set; } = new List<Package>();
}
