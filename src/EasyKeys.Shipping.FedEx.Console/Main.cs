using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.AddressValidation;
using EasyKeys.Shipping.FedEx.Rates;
using EasyKeys.Shipping.FedEx.Shipment;

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
        _applicationLifetime.ApplicationStopping.ThrowIfCancellationRequested();

        // var address2 = new ValidateAddress(
        //                Guid.NewGuid().ToString(),
        //                new Shipping.Abstractions.Address(
        //                "One Microsoft Way",
        //                "",
        //                "Redmond",
        //                "Washington",
        //                "98052",
        //                "US",
        //                false));

        // var address2 = new ValidateAddress(
        //           Guid.NewGuid().ToString(),
        //           new Shipping.Abstractions.Address(
        //           "Mauerberger  Building",
        //           "2nd floor",
        //           "Technion City",
        //           "Haifa",
        //           "3200003",
        //           "IL",
        //           false));

        // var address3 = new ValidateAddress(
        //       Guid.NewGuid().ToString(),
        //       new Shipping.Abstractions.Address(
        //       "100 East Capitol Street",
        //       "Suite 1000",
        //       "Jackson",
        //       "MS",
        //       "39201",
        //       "US",
        //       false));
        var address3 = new ValidateAddress(
           Guid.NewGuid().ToString(),
           new Shipping.Abstractions.Address(
           "5925 sycamore canyon blvd",
           "Apt 2105",
           "Riverside",
           "CA",
           "92507",
           "US",
           false));
        var address2 = new ValidateAddress(
            Guid.NewGuid().ToString(),
            new Shipping.Abstractions.Address(
            "1500 S STRONG DR",
            "Apartment, suite, unit, etc. (optional)",
            "BLOOMINGTON",
            "IL",
            "47403-8741",
            "US",
            false));

        var result2 = await _validationClient.ValidateAddressAsync(address2);
        result2.ValidationBag.TryGetValue("State", out var v);
        _logger.LogInformation("{isVerified}", v);

        var shipment = new Shipping.Abstractions.Models.Shipment(
            originAddress: address2?.ProposedAddress,
            destinationAddress: address3?.ProposedAddress,

            // A multiple - package shipment(MPS) consists of two or more packages shipped to the same recipient.
            // The first package in the shipment request is considered the master package.
            // To create a multiple - package shipment,
            // • Include the shipment level information such as TotalWeight, PackageCount, SignatureOptions)
            // on the master package. The SequenceID for this package is 1.
            // • In the master package reply, assign the tracking number of the first package in the
            // MasterTrackingID element for all subsequent packages.You must return the master tracking
            // number and increment the package number(SequenceID) for subsequent packages
            new List<Shipping.Abstractions.Package>()
            {
                new Shipping.Abstractions.Package(
                    new Shipping.Abstractions.Dimensions()
                {
                    Height = 20.00M,
                    Width = 20.00M,
                    Length = 20.00M
                }, 125.0M),
               new Shipping.Abstractions.Package(
                    new Shipping.Abstractions.Dimensions()
                {
                    Height = 10.00M,
                    Width = 10.00M,
                    Length = 10.00M
                }, 80.0M)
            },
            options: new ShipmentOptions()
            {
                PackagingType = FedExPackageType.YOUR_PACKAGING.ToString(),
                DeliverySignatureOptions = "NoSignatureRequired",
                SaturdayDelivery = true
            })
        {
            Shipper = new Contact
            {
                FullName = "keiko moffett",
                CompanyName = "EasyKeys.com",
                Email = "keiko@easykeys.com",
                PhoneNumber = "888-888-8888"
            },
            Recipient = new Contact
            {
                FullName = "Ed Moicoachv",
                CompanyName = "companyname",
                Email = "moincoachv@easykeys.com",
                PhoneNumber = "444-444-4444"
            }
        };

        var options = new LabelOptions()
        {
            PaymentType = "sender",
            LabelFormatType = "COMMON2D",
            ImageType = "PNG",
            CollectOnDelivery = new CollectOnDelivery() { Activated = true, Amount = 250, CollectionType = "guaranteed_funds", Currency = "USD" }
        };
        var rates = await _fedexRateProvider.GetRatesAsync(
            shipment,
            ServiceType.FEDEX_2_DAY);

        var result = await _fedExShipmentProvider.ProcessShipmentAsync(
            ServiceType.FEDEX_2_DAY,
            shipment,
            options);
        // save bytes
        return await Task.FromResult(0);
    }
}
