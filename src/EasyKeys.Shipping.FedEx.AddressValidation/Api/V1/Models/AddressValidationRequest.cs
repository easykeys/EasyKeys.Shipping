namespace EasyKeys.Shipping.FedEx.AddressValidation.Api.V1.Models;
internal class AddressValidationRequest
{
    public string InEffectAsOfTimestamp { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

    public ValidateAddressControlParameters ValidateAddressControlParameters { get; set; } = new();

    public List<AddressToValidate> AddressesToValidate { get; set; } = new();
}

internal class ValidateAddressControlParameters
{
    public bool IncludeResolutionTokens { get; set; } = true;
}

internal class AddressToValidate
{
    public Address Address { get; set; } = new();

    public string ClientReferenceId { get; set; } = Guid.NewGuid().ToString();
}

internal class Address
{
    public string[] StreetLines { get; set; } = Array.Empty<string>();

    public string City { get; set; } = string.Empty;

    public string StateOrProvinceCode { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string CountryCode { get; set; } = string.Empty;
}
