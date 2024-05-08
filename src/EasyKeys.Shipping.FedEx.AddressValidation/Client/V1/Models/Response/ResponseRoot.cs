using System.Text.Json.Serialization;

using EasyKeys.Shipping.FedEx.Abstractions.Models;

namespace EasyKeys.Shipping.FedEx.AddressValidation.Client.V1.Models.Response;

public class ResponseRoot
{
    [JsonPropertyName("transactionId")]
    public string? TransactionId { get; set; }

    [JsonPropertyName("customerTransactionId")]
    public string? CustomerTransactionId { get; set; }

    [JsonPropertyName("output")]
    public Output? Output { get; set; }

    [JsonPropertyName("errors")]
    public List<ApiError> Errors { get; set; } = new ();

    public bool IsValidAddress()
    {
        if (Output == null || Output.ResolvedAddresses == null)
        {
            return false;
        }

        foreach (var address in Output.ResolvedAddresses)
        {
            // Checking primary validation criteria
            var isStandardized = address.Attributes.TryGetValue("AddressType", out var addressType) && addressType.ToString() == "STANDARDIZED";
            var isDPVTrue = address.Attributes.TryGetValue("DPV", out var dpv) && dpv.ToString() == "true";
            var isInterpolated = address.Attributes.TryGetValue("InterpolatedStreetAddress", out var interpolated) && interpolated.ToString() == "true";

            // Check for specific customer message indicating invalidity
            var hasInvalidCustomerMessage = address.CustomerMessage?.Any(cm => cm.Value == "INTERPOLATED.STREET.ADDRESS") ?? false;

            // Check if additional validation attributes meet the required conditions
            var buildingValidated = address.Attributes.TryGetValue("BuildingValidated", out var buildingValid) && buildingValid.ToString() == "true";
            var postalValidated = address.Attributes.TryGetValue("PostalValidated", out var postalValid) && postalValid.ToString() == "true";

            // Final validation logic
            if (isStandardized && isDPVTrue && !isInterpolated && !hasInvalidCustomerMessage)
            {
                return true;
            }
        }

        return false;
    }
}
