namespace EasyKeys.Shipping.FedEx.AddressValidation.Api.V1.Models;

internal class AddressValidationResponse
{
    public string TransactionId { get; set; }

    public string CustomerTransactionId { get; set; }

    public Output Output { get; set; }

    public bool IsValidAddress()
    {
        if (Output == null || Output.ResolvedAddresses == null)
            return false;

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

internal class Output
{
    public List<ResolvedAddress> ResolvedAddresses { get; set; }

    public List<Alert> Alerts { get; set; }
}

internal class ResolvedAddress
{
    public string[] StreetLinesToken { get; set; }

    public string City { get; set; }

    public string StateOrProvinceCode { get; set; }

    public string CountryCode { get; set; }

    public List<CustomerMessage>? CustomerMessage { get; set; }

    public List<ResolutionToken> CityToken { get; set; }

    public ResolutionToken PostalCodeToken { get; set; }

    public ParsedPostalCode ParsedPostalCode { get; set; }

    public string Classification { get; set; }

    public bool PostOfficeBox { get; set; }

    public bool NormalizedStatusNameDPV { get; set; }

    public string StandardizedStatusNameMatchSource { get; set; }

    public string ResolutionMethodName { get; set; }

    public bool RuralRouteHighwayContract { get; set; }

    public bool GeneralDelivery { get; set; }

    public Dictionary<string, object> Attributes { get; set; }
}

internal class CustomerMessage
{
    public bool Changed { get; set; }

    public string Value { get; set; }
}

internal class ResolutionToken
{
    public bool Changed { get; set; }

    public string Value { get; set; }
}

internal class ParsedPostalCode
{
    public string Base { get; set; }

    public string AddOn { get; set; }

    public string DeliveryPoint { get; set; }
}

internal class Alert
{
    public string Code { get; set; }

    public string Message { get; set; }

    public string AlertType { get; set; }
}
