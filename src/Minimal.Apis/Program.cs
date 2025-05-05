using System.Text.Json;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.AddressValidation;
using EasyKeys.Shipping.FedEx.Rates;
using EasyKeys.Shipping.FedEx.Shipment;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.AddressValidation;
using EasyKeys.Shipping.Stamps.Rates;
using EasyKeys.Shipping.Stamps.Rates.Models;
using EasyKeys.Shipping.Stamps.Shipment;
using EasyKeys.Shipping.Stamps.Shipment.DependencyInjection;
using EasyKeys.Shipping.Stamps.Shipment.Models;
using EasyKeys.Shipping.Stamps.Tracking;
using EasyKeys.Shipping.Stamps.Tracking.DependencyInjection;

using Minimal.Apis.Models;

var builder = WebApplication.CreateBuilder(args);

// retrieve values from azure vault
builder.Configuration.AddAzureKeyVault(hostingEnviromentName: builder.Environment.EnvironmentName, usePrefix: true);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// add stamps libraries
builder.Services.AddStampsAddressProvider();

builder.Services.AddStampsRateProvider();

builder.Services.AddStampsShipmentProvider();

builder.Services.AddStampsTrackingProvider();

// add fedex libraries
builder.Services.AddWebServicesFedExAddressValidationProvider();

builder.Services.AddWebServicesFedExRateProvider();

builder.Services.AddWebServicesFedExShipmenProvider();

builder.Services.AddFedExTrackingProvider();

// configure json options
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.IncludeFields = true;
});

var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

// address validation recieves a proposed address.
app.MapPost("/stamps/addressValidation", async (
    AddressValidationDto model,
    IStampsAddressValidationProvider addressProvider,
    CancellationToken cancellationToken) =>
{
    var address = new ValidateAddress(model.Id, model.Address);
    var validatedAddress = await addressProvider.ValidateAddressAsync(address, cancellationToken);
    app.Logger.LogInformation("test");
    return Results.Json(validatedAddress, options);
})
.Accepts<ShipmentDto>("application/json")
.Produces<ValidateAddress>(StatusCodes.Status200OK, "application/json")
.WithName("StampsAddressValidation");

app.MapPost("/fedex/addressValidation", async (
    AddressValidationDto model,
    IFedExAddressValidationProvider addressProvider,
    CancellationToken cancellationToken) =>
{
    var address = new ValidateAddress(model.Id, model.Address);
    var validatedAddress = await addressProvider.ValidateAddressAsync(address, cancellationToken);

    return Results.Json(validatedAddress, options);
})
.Accepts<ShipmentDto>("application/json")
.Produces<ValidateAddress>(StatusCodes.Status200OK, "application/json")
.WithName("FedExAddressValidation");

// getRates recieves a rate model containing destination address and package information.
app.MapPost("/stamps/getRates", async (
    StampsShipmentDto model,
    IStampsRateProvider rateProvider,
    CancellationToken cancellationToken) =>
{
    var listOfRates = new List<Rate>();

    var package = new Package(
    model!.Package!.Length,
    model.Package.Width,
    model.Package.Height,
    model.Package.Weight,
    model.Package.InsuredValue,
    model.Package.SignatureRequiredOnDelivery);

    var configurator = new StampsRateConfigurator(
        model.Origin!,
        model.Destination!,
        package,
        model.Package.ShipDate);

    var rateOptions = new RateOptions
    {
        Sender = model.Sender,
        Recipient = model.Recipient!,
    };

    foreach (var shipment in configurator.Shipments)
    {
        if (shipment?.DestinationAddress?.IsUnitedStatesAddress() ?? false)
        {
            rateOptions.DeclaredValue = model.Package.InsuredValue;
        }

        var result = await rateProvider.GetRatesAsync(shipment!, rateOptions, cancellationToken);
        foreach (var rate in result.Rates)
        {
            var found = listOfRates.FirstOrDefault(x => x.Name == rate.Name && x.PackageType == rate.PackageType);
            if (found is null)
            {
                listOfRates.Add(rate);
            }
        }
    }

    return Results.Json(listOfRates.OrderBy(x => x.Name).ThenBy(x => x.TotalCharges), options);
})
.Accepts<StampsShipmentDto>("application/json")
.Produces<Shipment>(StatusCodes.Status200OK, "application/json")
.WithName("StampsGetRates");

app.MapPost("/fedex/getRates", async (
    ShipmentDto model,
    IFedExRateProvider rateProvider,
    CancellationToken cancellationToken) =>
{
    // create a package
    var defaultPackage = new Package(
        model!.Package!.Length,
        model.Package.Width,
        model.Package.Height,
        model.Package.Weight,
        model.Package.InsuredValue,
        model.Package.SignatureRequiredOnDelivery);

    var config = new FedExRateConfigurator(model.Origin, model?.Destination!, defaultPackage);

    var result = new List<Rate>();

    foreach (var (shipment, serviceType) in config.Shipments)
    {
        var response = await rateProvider.GetRatesAsync(shipment, serviceType, cancellationToken);

        foreach (var err in response.Errors)
        {
            app.Logger.LogError("{num}{desc}", err.Number, err.Description);
        }

        result.AddRange(response.Rates);
    }

    return Results.Json(result.OrderBy(x => x.TotalCharges), options);
})
.Accepts<ShipmentDto>("application/json")
.Produces<Shipment>(StatusCodes.Status200OK, "application/json")
.WithName("FedExGetRates");

// create the shipment when rates service type is selected.
app.MapPost("/stamps/createShipment", async (
    StampsShipmentDto model,
    string serviceType,
    string packageType,
    string orderId,
    bool isSample,
    IStampsShipmentProvider shipmentProvider,
    CancellationToken cancellationToken) =>
{
    var package = new Package(
        model!.Package!.Length,
        model.Package.Width,
        model.Package.Height,
        model.Package.Weight,
        model.Package.InsuredValue,
        model.Package.SignatureRequiredOnDelivery);

    var configurator = new StampsRateConfigurator(
        model.Origin!,
        model.Destination!,
        package,
        model.Package.ShipDate);

    var correctShipment = configurator.Shipments.Where(x => x.Options.PackagingType == packageType).FirstOrDefault();

    var shipmentDetails = new ShipmentDetails();
    shipmentDetails.LabelOptions.Memo = orderId;
    shipmentDetails.IsSample = isSample;

    if (correctShipment == null)
    {
        return Results.Json($"No Shipment Found with PackageType: {packageType}");
    }

    var rateOptions = new RateOptions
    {
        Sender = model.Sender,
        Recipient = model!.Recipient!,
        ServiceType = StampsServiceType.FromName(serviceType)
    };

    var labels = await shipmentProvider.CreateShipmentAsync(correctShipment, rateOptions, shipmentDetails, cancellationToken);

    foreach (var label in labels.Labels)
    {
        await File.WriteAllBytesAsync($"{label.ProviderLabelId}.{label.ImageType}", label.Bytes[0], cancellationToken);
    }

    return Results.Json(labels, options);
})
.Accepts<StampsShipmentDto>("application/json")
.Produces<ShipmentLabel>(StatusCodes.Status200OK, "application/json")
.WithName("StampsCreateShipment");

// create the shipment when rates service type is selected.
app.MapPost("/fedex/createShipment", async (
    FedExShipmentDto model,
    string serviceType,
    string packageType,
    string orderId,
    IFedExShipmentProvider shipmentProvider,
    CancellationToken cancellationToken) =>
{
    var package = new Package(
        model!.Package!.Length,
        model.Package.Width,
        model.Package.Height,
        model.Package.Weight,
        model.Package.InsuredValue,
        model.Package.SignatureRequiredOnDelivery);

    var configurator = new FedExRateConfigurator(
        model.Origin!,
        model.Destination!,
        package,
        true,
        model.Package.ShipDate);

    var stype = FedExServiceType.FromName(serviceType);
    var ptype = FedExPackageType.FromName(packageType);

    var shipmentOptions = new ShipmentOptions(ptype.Name, model?.Package?.ShipDate ?? DateTime.Now);
    var packages = new List<Package>
    {
        package
    };

    var correctShipment = new Shipment(model!.Origin, model.Destination, packages, shipmentOptions);

    var shipmentDetails = new EasyKeys.Shipping.FedEx.Shipment.Models.ShipmentDetails
    {
        Sender = model.Sender,
        Recipient = model!.Recipient!,

        TransactionId = orderId,

        PaymentType = FedExPaymentType.Sender,

        RateRequestType = "list",

        // CollectOnDelivery = cod,
        // AU with economy is not working
        // DeliverySignatureOptions = "NoSignatureRequired",
        LabelOptions = new EasyKeys.Shipping.FedEx.Shipment.Models.LabelOptions()
        {
            LabelFormatType = "COMMON2D",
            ImageType = "PNG",
        }
    };

    if (model?.Commodity != null)
    {
        shipmentDetails.Commodities.Add(model.Commodity);
    }

    if (correctShipment == null)
    {
        return Results.Json($"No Shipment Found: {serviceType}");
    }

    var labels = await shipmentProvider.CreateShipmentAsync(stype, correctShipment, shipmentDetails, cancellationToken);

    foreach (var label in labels.Labels)
    {
        await File.WriteAllBytesAsync($"{label.ProviderLabelId}.{label.ImageType}", label.Bytes[0], cancellationToken);
    }

    return Results.Json(labels, options);
})
.Accepts<FedExShipmentDto>("application/json")
.Produces<ShipmentLabel>(StatusCodes.Status200OK, "application/json")
.WithName("FedExCreateShipment");

app.MapPost("/stamps/createInternationalShipment", async (
    InternationalShipmentDto model,
    string serviceType,
    string packageType,
    string orderId,
    bool isSample,
    IStampsShipmentProvider shipmentProvider,
    CancellationToken cancellationToken) =>
{
    var package = new Package(
        model!.Package!.Length,
        model.Package.Width,
        model.Package.Height,
        model.Package.Weight,
        model.Package.InsuredValue,
        model.Package.SignatureRequiredOnDelivery);

    var configurator = new StampsRateConfigurator(
        model.Origin!,
        model.Destination!,
        package,
        model.Package.ShipDate);

    var correctShipment = configurator.Shipments.Where(x => x.Options.PackagingType == packageType).FirstOrDefault();

    var shipmentDetails = new ShipmentDetails();
    shipmentDetails.LabelOptions.Memo = orderId;
    shipmentDetails.IsSample = isSample;
    shipmentDetails.Commodities.Add(model.Commodity!);
    shipmentDetails.CustomsInformation.InvoiceNumber = orderId;
    shipmentDetails.CustomsInformation.CustomsSigner = "Easykeys.com employee";

    shipmentDetails.LabelOptions.Memo = "This will be orderId";

    var rateOptions = new RateOptions
    {
        Sender = model.Sender,
        Recipient = model.Recipient!,
        ServiceType = StampsServiceType.FromName(serviceType),
        DeclaredValue = model!.Commodity!.Amount!,
    };

    if (correctShipment == null)
    {
        return Results.Json($"No Shipment Found with PackageType: {packageType}");
    }

    var label = await shipmentProvider.CreateShipmentAsync(
        correctShipment,
        rateOptions,
        shipmentDetails,
        cancellationToken);

    return Results.Json(label, options);
})
.Accepts<InternationalShipmentDto>("application/json")
.Produces<ShipmentLabel>(StatusCodes.Status200OK, "application/json")
.WithName("StampsCreateInternationalShipment");

// track shipment after it is created.
app.MapGet("/stamps/trackShipment/{id}", async (
    string id,
    IStampsTrackingProvider trackingProvider,
    CancellationToken cancellationToken) =>
{
    var trackingInfo = await trackingProvider.TrackShipmentAsync(id, cancellationToken);

    return Results.Json(trackingInfo, options);
});

// cancel a label after it is created.
app.MapDelete("/stamps/cancelShipment/{id}", async (
    string id,
    IStampsShipmentProvider shipmentProvider,
    CancellationToken cancellationToken) =>
{
    var result = await shipmentProvider.CancelShipmentAsync(id, cancellationToken);

    return Results.Json(result, options);
});

await app.RunAsync();
