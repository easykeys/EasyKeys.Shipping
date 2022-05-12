using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.API.Models;

public class RateQuoteDto
{
    public Address? Origin { get; set; }

    public Address? Destination { get; set; }

    public PackageDto? Package { get; set; }

}
