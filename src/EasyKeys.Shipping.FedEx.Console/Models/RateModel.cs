using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.FedEx.Console.Models;

public class RateModel
{
    public Address Address { get; set; } = new Address();

    public IList<Package> Packages { get; set; } = new List<Package>();
}
