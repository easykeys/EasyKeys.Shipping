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

        public async Task<List<RateV40>> GetRatesAsync(Shipment shipment, CancellationToken cancellationToken = default)
        {
            var stampsClient = _stampsClient.CreateClient();

            var request = new GetRatesRequest()
            {
                Item = await _stampsClient.GetTokenAsync(cancellationToken),

                Rate = new RateV40()
                {
                    From = new StampsClient.v111.Address()
                    {
                        FullName = shipment.SenderInformation.FullName,

                        Address1 = shipment.OriginAddress.StreetLine,

                        State = shipment.OriginAddress.StateOrProvince,

                        ZIPCode = shipment.OriginAddress.PostalCode,

                        EmailAddress = shipment.SenderInformation.Email
                    },

                    To = new StampsClient.v111.Address()
                    {
                        FullName = shipment.RecipientInformation.FullName,

                        Address1 = shipment.DestinationAddress.StreetLine,

                        State = shipment.DestinationAddress.StateOrProvince,

                        PostalCode = shipment.DestinationAddress.PostalCode,

                        ZIPCode = shipment.DestinationAddress.PostalCode,

                        EmailAddress = shipment.RecipientInformation.Email
                    },

                    Amount = 0.0m,

                    MaxAmount = 0.0m,

                    ServiceType = ServiceType.Unknown,

                    ServiceDescription = String.Empty,

                    PrintLayout = string.Empty,

                    DeliverDays = string.Empty,

                    WeightLb = (double)shipment.Packages.Sum(x => x.Weight),

                    WeightOz = 0.0,

                    PackageType = PackageTypeV11.Package,

                    //RequiresAllOf =

                    Length = 1.0d,

                    Width = 1.0d,

                    Height = 1.0d,

                    ShipDate = DateTime.Now,

                    //DeliveryDate =

                    InsuredValue = 100.0m,

                    RegisteredValue = 0.0m,

                    CODValue = 0.0m,

                    DeclaredValue = 0.0m,

                    NonMachinable = false,

                    RectangularShaped = true,

                    Prohibitions = String.Empty,

                    Restrictions = String.Empty,

                    Observations = String.Empty,

                    Regulations = String.Empty,

                    GEMNotes = String.Empty,

                    MaxDimensions = String.Empty,

                    DimWeighting = String.Empty,

                    //AddOns =

                    //Surcharges =

                    EffectiveWeightInOunces = 0,

                    Zone = 0,

                    RateCategory = 0,

                    CubicPricing = false,

                    ContentType = ContentTypeV2.Other,

                    EntryFacility = EntryFacilityV1.Unknown,

                    SortType = SortTypeV1.Unknown,
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

                return response.Rates.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
