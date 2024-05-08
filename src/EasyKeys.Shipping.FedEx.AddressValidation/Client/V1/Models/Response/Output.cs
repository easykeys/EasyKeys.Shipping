namespace EasyKeys.Shipping.FedEx.AddressValidation.Client.V1.Models.Response;

public class Output
{
    public List<ResolvedAddress>? ResolvedAddresses { get; set; }

    public List<Alert>? Alerts { get; set; }
}
