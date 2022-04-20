using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Services;

using Microsoft.Extensions.Logging;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Rates
{
    public class StampsRateProvider : IStampsRateProvider
    {
        private readonly IStampsClientService _stampsClient;
        private readonly ILogger<StampsRateProvider> _logger;

        public StampsRateProvider(IStampsClientService stampsClientService, ILogger<StampsRateProvider> logger)
        {
            _stampsClient = stampsClientService;
            _logger = logger;
        }

        public async Task<GetRatesResponse> GetRatesAsync(Shipment shipment, CancellationToken cancellationToken = default)
        {

            var stampsClient = _stampsClient.CreateClient();

            var request = new GetRatesRequest()
            {
                Item = await _stampsClient.GetTokenAsync(cancellationToken),

                Rate = new RateV40()
                {
                    From = new StampsClient.v111.Address()
                    {
                        FullName = "EasyKeys",

                        Address1 = shipment.OriginAddress.StreetLine,

                        State = shipment.OriginAddress.StateOrProvince,

                        ZIPCode = shipment.OriginAddress.PostalCode,

                        EmailAddress = "bmoffett@easykeys.com"
                    },
                    To = new StampsClient.v111.Address()
                    {
                        FullName = "Brandon Moffett",

                        Address1 = shipment.DestinationAddress.StreetLine,

                        State = shipment.DestinationAddress.StateOrProvince,

                        PostalCode = shipment.DestinationAddress.PostalCode,

                        ZIPCode = shipment.DestinationAddress.PostalCode,

                        EmailAddress = "ucrengineerpy@gmail.com"
                    },
                    WeightLb = (double)shipment.Packages.Sum(x => x.Weight),

                    WeightOz = 0.0,

                    PackageType = PackageTypeV11.Package,

                    ShipDate = DateTime.Now,

                    ContentType = ContentTypeV2.Other
                },
                Carrier = Carrier.USPS
            };

            try
            {
                var response = await stampsClient.GetRatesAsync(request);

                foreach (var rate in response.Rates)
                {
                    var addons = rate.AddOns.Select(x => x.AddOnDescription).Flatten(",");

                    var required = rate.RequiresAllOf?.Length;

                    rate.InsuredValue = 100M;

                    rate.AddOns = null;

                    _logger.LogInformation($"{rate.ServiceType} : {rate.ServiceDescription}");

                    _logger.LogInformation($" => Addons Available : {addons}");

                    _logger.LogInformation($" => Required Addons : {rate.RequiresAllOf?.ToString()}");
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
