using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Response;

public class OperationalDetail
{
    [JsonPropertyName("ineligibleForMoneyBackGuarantee")]
    public bool? IneligibleForMoneyBackGuarantee { get; set; }

    [JsonPropertyName("astraDescription")]
    public string? AstraDescription { get; set; }

    [JsonPropertyName("airportId")]
    public string? AirportId { get; set; }

    [JsonPropertyName("serviceCode")]
    public string? ServiceCode { get; set; }

    [JsonPropertyName("originLocationIds")]
    public string[]? OriginLocationIds { get; set; }

    [JsonPropertyName("commitDays")]
    public string[]? CommitDays { get; set; }

    [JsonPropertyName("scac")]
    public string? Scac { get; set; }

    [JsonPropertyName("originServiceAreas")]
    public string[]? OriginServiceAreas { get; set; }

    [JsonPropertyName("deliveryDay")]
    public string? DeliveryDay { get; set; }

    [JsonPropertyName("originLocationNumbers")]
    public double[]? OriginLocationNumbers { get; set; }

    [JsonPropertyName("destinationPostalCode")]
    public string? DestinationPostalCode { get; set; }

    [JsonPropertyName("commitDate")]
    public DateTime? CommitDate { get; set; }

    [JsonPropertyName("deliveryDate")]
    public string? DeliveryDate { get; set; }

    [JsonPropertyName("deliveryEligibilities")]
    public string? DeliveryEligibilities { get; set; }

    [JsonPropertyName("maximumTransitTime")]
    public string? MaximumTransitTime { get; set; }

    [JsonPropertyName("astraPlannedServiceLevel")]
    public string? AstraPlannedServiceLevel { get; set; }

    [JsonPropertyName("destinationLocationIds")]
    public string[]? DestinationLocationIds { get; set; }

    [JsonPropertyName("destinationLocationStateOrProvinceCodes")]
    public string[]? DestinationLocationStateOrProvinceCodes { get; set; }

    [JsonPropertyName("transitTime")]
    public string? TransitTime { get; set; }

    [JsonPropertyName("packagingCode")]
    public string? PackagingCode { get; set; }

    [JsonPropertyName("destinationLocationNumbers")]
    public double[]? DestinationLocationNumbers { get; set; }

    [JsonPropertyName("publishedDeliveryTime")]
    public string? PublishedDeliveryTime { get; set; }

    [JsonPropertyName("countryCodes")]
    public string[]? CountryCodes { get; set; }

    [JsonPropertyName("stateOrProvinceCodes")]
    public string[]? StateOrProvinceCodes { get; set; }

    [JsonPropertyName("ursaPrefixCode")]
    public string? UrsaPrefixCode { get; set; }

    [JsonPropertyName("ursaSuffixCode")]
    public string? UrsaSuffixCode { get; set; }

    [JsonPropertyName("destinationServiceAreas")]
    public string[]? DestinationServiceAreas { get; set; }

    [JsonPropertyName("originPostalCodes")]
    public string[]? OriginPostalCodes { get; set; }

    [JsonPropertyName("customTransitTime")]
    public string? CustomTransitTime { get; set; }
}
