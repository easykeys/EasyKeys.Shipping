using EasyKeys.Shipping.Abstractions.Models;

namespace Models;

public class RateModelDto
{
    public Commodity? Commodity { get; set; }

    public ContactInfo Contact { get; set; } = new ContactInfo();

    public Address Address { get; set; } = new Address();

    public IList<Package> Packages { get; set; } = new List<Package>();
}
