using System.Threading;
using System.Threading.Tasks;

using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.FedEx.Rates
{
    public interface IFedExRateProvider
    {
        /// <summary>
        /// Retrieves FedEx rates for non ground services.
        /// </summary>
        /// <param name="shipment">The shipment with packages.</param>
        /// <param name="serviceType">The type of fedex service to be utilized.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<Shipment> GetRatesAsync(Shipment shipment, ServiceType serviceType = ServiceType.DEFAULT, CancellationToken cancellationToken = default);
    }
}
