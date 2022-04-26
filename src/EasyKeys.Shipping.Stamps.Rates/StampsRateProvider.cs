using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Services;

using Microsoft.Extensions.Logging;

namespace EasyKeys.Shipping.Stamps.Rates
{
    public class StampsRateProvider : IStampsRateProvider
    {
        private readonly IStampsClientService _stampsClient;
        private readonly IRatesService _ratesService;
        private readonly ILogger<StampsRateProvider> _logger;

        public StampsRateProvider(IStampsClientService stampsClientService, IRatesService ratesService, ILogger<StampsRateProvider> logger)
        {
            _stampsClient = stampsClientService;
            _ratesService = ratesService;
            _logger = logger;
        }

        public async Task<Shipment> GetRatesAsync(Shipment shipment, RateRequestDetails rateRequestDetails, CancellationToken cancellationToken = default)
        {

            var rates = await _ratesService.GetRatesResponseAsync(shipment, rateRequestDetails, cancellationToken);

            foreach (var rate in rates)
            {
                shipment.Rates.Add(new Shipping.Abstractions.Rate($"{rate.ServiceType}", rate.ServiceDescription, rate.Amount, rate.DeliveryDate));

                _logger.LogInformation($"{rate.ServiceType} : {rate.ServiceDescription}");

                _logger.LogInformation($" => Cost : {rate.Amount}");

                _logger.LogInformation($" => Delivery Days : {rate.DeliverDays}");
            }

            return shipment;
        }
    }
}
