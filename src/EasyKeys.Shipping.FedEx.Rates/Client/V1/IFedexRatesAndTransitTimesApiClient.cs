using EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;
using EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Response;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1;

/// <summary>
/// Implmentation of the FedEx Rates and Transit Times client. Can be used to get rates and transit times.
/// <see href="https://developer.fedex.com/api/en-us/catalog/rate/v1/docs.html"/>.
/// </summary>
public interface IFedexRatesAndTransitTimesClient
{
    Task<ResponseRoot> GetRatesAsync(RequestRoot request, CancellationToken cancellationToken);
}
