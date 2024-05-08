using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class Contact
{
    [JsonPropertyName("companyName")]
    public string? CompanyName { get; set; }

    [JsonPropertyName("faxNumber")]
    public string? FaxNumber { get; set; }

    [JsonPropertyName("personName")]
    public string? PersonName { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("emailAddress")]
    public string? EmailAddress { get; set; }

    [JsonPropertyName("phoneExtension")]
    public string? PhoneExtension { get; set; }
}
