using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.API.Models;

public class RateModel
{
    public Address Origin { get; set; } = new Address(
                                                streetLine: "11407 Granite Street",
                                                city: "Charlotte",
                                                stateOrProvince: "NC",
                                                postalCode: "28273",
                                                countryCode: "US");

    public string Length { get; set; } = string.Empty;

    public string Width { get; set; } = string.Empty;

    public string Height { get; set; } = string.Empty;

    public string Weight { get; set; } = string.Empty;
}
