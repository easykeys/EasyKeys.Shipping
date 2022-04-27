using System.Text.Json;

using Bet.Extensions;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.AddressValidation;
using EasyKeys.Shipping.FedEx.Rates;
using EasyKeys.Shipping.FedEx.Shipment;
using EasyKeys.Shipping.FedEx.Shipment.Models;

namespace EasyKeys.Shipping.FedEx.Console;

public class Main : IMain
{
    private readonly IFedExAddressValidationProvider _validationClient;
    private readonly IFedExRateProvider _fedexRateProvider;
    private readonly IFedExShipmentProvider _fedExShipmentProvider;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ILogger<Main> _logger;

    public Main(
        IFedExAddressValidationProvider validationClient,
        IFedExRateProvider fedExRateProvider,
        IFedExShipmentProvider fedExShipmentProvider,
        IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration,
        ILogger<Main> logger)
    {
        _validationClient = validationClient ?? throw new ArgumentNullException(nameof(validationClient));
        _fedexRateProvider = fedExRateProvider ?? throw new ArgumentNullException(nameof(fedExRateProvider));
        _fedExShipmentProvider = fedExShipmentProvider ?? throw new ArgumentNullException(nameof(fedExShipmentProvider));
        _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
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

        var packages = new List<Package>
        {
            new Package(
                new Dimensions()
                {
                    Height = 20.00M,
                    Width = 20.00M,
                    Length = 20.00M
                },
                125.0M),

            new Package(
                new Dimensions()
                {
                    Height = 10.00M,
                    Width = 10.00M,
                    Length = 10.00M
                },
                80.0M),

            // new Package(
            //    new Dimensions()
            //    {
            //        Height = 11.00M,
            //        Width = 11.00M,
            //        Length = 11.00M
            //    },
            //    40.0M)
        };

        var shipmentOptions = new ShipmentOptions
        {
            PackagingType = "YOUR_PACKAGING",
            SaturdayDelivery = true,
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
                ImageType = "PNG",
            }
        };

        var commodity = new Commodity()
        {
            Name = "Keys",
            NumberOfPieces = 3,
            Description = "A set of 4 Steel Keys for office furniture",
            CountryOfManufacturer = "US",
            Weight = 10.0M,
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

        using var stream = EmbeddedResource.GetAsStreamFromCallingAssembly("Embeded.Addresses.json");
        var addressList = JsonSerializer.Deserialize(stream, typeof(List<Address>)) as List<Address>;

        var count = 0;
        foreach (var address in addressList)
        {
            // 1. address validation

            // for international orders, user must enter the provice/state code, not full name
            var validateRequest = new ValidateAddress(count.ToString(), address);
            var validationResult = await _validationClient.ValidateAddressAsync(validateRequest, cancellationToken);
            count++;

            // state is verified
            _logger.LogInformation(validationResult.ValidationBag.Select(x => $"{x.Key}-{x.Value}").Flatten(","));

            var shipment = new Shipping.Abstractions.Models.Shipment(
                originAddress,
                validationResult?.ProposedAddress,
                packages,
                shipmentOptions);

            shipment.SenderInfo = sender;
            shipment.RecipientInfo = recipient;

            shipment.Commodities.Add(commodity);

            // 2. shipment rates
            var rates = await _fedexRateProvider.GetRatesAsync(
                shipment: shipment,
                serviceType: shipment.DestinationAddress.CountryCode == "US" ? ServiceType.FEDEX_2_DAY : ServiceType.INTERNATIONAL_ECONOMY,
                cancellationToken);

            if (shipment.DestinationAddress.CountryCode != "US")
            {
                details.CollectOnDelivery.Activated = false;
            }

            // 3. get shipment label
            var result = await _fedExShipmentProvider.CreateShipmentAsync(
                serviceType: shipment.DestinationAddress.CountryCode == "US" ? ServiceType.FEDEX_2_DAY : ServiceType.INTERNATIONAL_ECONOMY,
                shipment,
                details,
                cancellationToken);

            await File.WriteAllBytesAsync("label.png", result.Labels[0].Bytes[0]);
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
}
