using System.Text.Json;

using Bet.Extensions;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.AddressValidation;
using EasyKeys.Shipping.FedEx.Rates;
using EasyKeys.Shipping.FedEx.Shipment;
using EasyKeys.Shipping.FedEx.Shipment.Models;
using EasyKeys.Shipping.FedEx.Tracking;

using Models;

namespace EasyKeys.Shipping.FedEx.Console;

public class Main : IMain
{
    private readonly IFedExAddressValidationProvider _validationClient;
    private readonly IFedExRateProvider _fedexRateProvider;
    private readonly IFedExShipmentProvider _fedExShipmentProvider;
    private readonly IFedExTrackingProvider _fedExTrackingProvider;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ILogger<Main> _logger;

    public Main(
        IFedExAddressValidationProvider validationClient,
        IFedExRateProvider fedExRateProvider,
        IFedExShipmentProvider fedExShipmentProvider,
        IFedExTrackingProvider fedExTrackingProvider,
        IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration,
        ILogger<Main> logger)
    {
        _validationClient = validationClient ?? throw new ArgumentNullException(nameof(validationClient));
        _fedexRateProvider = fedExRateProvider ?? throw new ArgumentNullException(nameof(fedExRateProvider));
        _fedExShipmentProvider = fedExShipmentProvider ?? throw new ArgumentNullException(nameof(fedExShipmentProvider));
        _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        _fedExTrackingProvider = fedExTrackingProvider ?? throw new ArgumentNullException(nameof(fedExTrackingProvider));
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IConfiguration Configuration { get; set; }

    public async Task<int> RunAsync()
    {
        _logger.LogInformation("Main executed");

        // use this token for stopping the services
        var cancellationToken = _applicationLifetime.ApplicationStopping;

        var originAddress = new Address(
            streetLine: "11407 Granite Street",
            city: "Charlotte",
            stateOrProvince: "NC",
            postalCode: "28273",
            countryCode: "US");

        var shipmentOptions = new ShipmentOptions(FedExPackageType.YourPackaging.Name, DateTime.Now, true)
        {
            PreferredCurrencyCode = "USD",
        };

        var details = new ShipmentDetails
        {
            TransactionId = "1234-transaction",
            PaymentType = "sender",
            CollectOnDelivery = new CollectOnDelivery()
            {
                Activated = false,
                Amount = 250,
                CollectionType = "guaranteed_funds",
                Currency = "USD"
            },
            DeliverySignatureOptions = "NoSignatureRequired",
            LabelOptions = new LabelOptions()
            {
                LabelFormatType = "COMMON2D",
                ImageType = "PDF",
            }
        };

        var commodity = new Commodity()
        {
            Name = "Keys",
            NumberOfPieces = 3,
            Description = "A set of 4 Steel Keys for office furniture",
            CountryOfManufacturer = "US",
            Quantity = 3,
            QuantityUnits = "EA",
            UnitPrice = 15.0M
        };

        var sender = new ContactInfo
        {
            FirstName = "EasyKeys.com",
            LastName = "Customer Support",
            Company = "EasyKeys.com",
            PhoneNumber = "8778395397",
            Email = string.Empty
        };

        var recipient = new ContactInfo
        {
            FirstName = "Ed",
            LastName = "Moicoachv",
            Company = "companyname",
            Email = "moincoachv@easykeys.com",
            PhoneNumber = "444-444-4444"
        };

        //var fileName = "Embeded.intnl-addresses.json";
        var fileName = "Embeded.domestic-addresses.json";

        var models = LoadModels<List<RateModelDto>>(fileName);

        foreach (var model in models)
        {
            var proposedAddress = await ValidateAsync(model.Address, false, cancellationToken);
            var defaultPackage = new Package(model.Packages[0].Dimensions, model.Packages[0].Weight, 100m, false);

            var config = new FedExRateConfigurator(originAddress, proposedAddress.ProposedAddress, defaultPackage);

            _logger.LogInformation("Rates for: {address}", proposedAddress.ProposedAddress.ToString());

            foreach (var (shipment, serviceType) in config.Shipments)
            {
                // 2. shipment rates
                var rates = await _fedexRateProvider.GetRatesAsync(shipment, serviceType, cancellationToken);
                if (rates.Errors.Any())
                {
                    var errors = rates.Errors.Select(x => x.Description).Flatten(",");
                    _logger.LogError(errors);
                }

                foreach (var rate in rates.Rates)
                {
                    _logger.LogInformation("{serviceName} - {name} - {cost}", rate.ServiceName, rate.Name, rate.TotalCharges2);
                }
            }

            var shipmentDetails = new ShipmentDetails
            {
                Sender = sender,
                Recipient = recipient,
            };

            shipmentDetails.Commodities.Add(commodity);

            //if (shipment.DestinationAddress.CountryCode != "US")
            //{
            //    details.CollectOnDelivery.Activated = false;
            //}

            //// 3. get shipment label
            //var result = await _fedExShipmentProvider.CreateShipmentAsync(
            //    serviceType: shipment.DestinationAddress.CountryCode == "US" ? FedExServiceType.FedExSecondDay : FedExServiceType.FedExInternationalEconomy,
            //    shipment,
            //    details,
            //    cancellationToken);

            //await File.WriteAllBytesAsync("label.png", result.Labels[0].Bytes[0]);

            //var info = await _fedExTrackingProvider.TrackShipmentAsync(result, cancellationToken);
        }

        // A multiple - package shipment(MPS) consists of two or more packages shipped to the same recipient.
        // The first package in the shipment request is considered the master package.
        // To create a multiple - package shipment,
        // • Include the shipment level information such as TotalWeight, PackageCount, SignatureOptions)
        // on the master package. The SequenceID for this package is 1.
        // • In the master package reply, assign the tracking number of the first package in the
        // MasterTrackingID element for all subsequent packages.You must return the master tracking
        // number and increment the package number(SequenceID) for subsequent packages
        return 0;
    }

    private static T LoadModels<T>(string fileName) where T : class
    {
        using var stream = EmbeddedResource.GetAsStreamFromCallingAssembly(fileName);
        var models = JsonSerializer.Deserialize(stream, typeof(T), new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip }) as T;

        return models;
    }

    private async Task<ValidateAddress> ValidateAsync(Address destination, bool debug = false, CancellationToken cancellationToken = default)
    {
        // 1. address validation
        // for international orders, user must enter the provice/state code, not full name
        var request = new ValidateAddress(Guid.NewGuid().ToString(), destination);
        var result = await _validationClient.ValidateAddressAsync(request, cancellationToken);

        // var originalAddress = JsonSerializer.Serialize(validationResult.OriginalAddress, new JsonSerializerOptions() { WriteIndented = true });
        // _logger.LogInformation(originalAddress);
        if (request.OriginalAddress != request.ProposedAddress && debug)
        {
            var propsedAddress = JsonSerializer.Serialize(result.ProposedAddress, new JsonSerializerOptions() { WriteIndented = true });
            _logger.LogInformation(propsedAddress);
        }

        if (debug)
        {
            // 1.a display validation bag
            _logger.LogInformation(Environment.NewLine + result.ValidationBag.Select(x => $"{x.Key}-{x.Value}").Flatten(Environment.NewLine));
        }

        return result;
    }
}
