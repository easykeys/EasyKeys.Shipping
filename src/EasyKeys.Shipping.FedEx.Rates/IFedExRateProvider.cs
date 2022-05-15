using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;

namespace EasyKeys.Shipping.FedEx.Rates;

/// <summary>
/// <para>Provides the implementation of the FedEx shipping rate provider.</para>
/// <para>Live calculator <see href="https://www.fedex.com/en-us/online/rating.html#"/>.</para>
/// </summary>
public interface IFedExRateProvider
{
    /// <summary>
    /// Retrieves FedEx Shipment Rates for specified <see cref="FedExServiceType"/>.
    /// </summary>
    /// <param name="shipment">The shipment with packages.</param>
    /// <param name="serviceType">The type of fedex service to be utilized.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<Shipment> GetRatesAsync(Shipment shipment, FedExServiceType? serviceType = null, CancellationToken cancellationToken = default);
}
