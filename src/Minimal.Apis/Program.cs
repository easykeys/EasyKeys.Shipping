using System.Text.Json;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.AddressValidation;
using EasyKeys.Shipping.FedEx.Rates;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.AddressValidation;
using EasyKeys.Shipping.Stamps.Rates;
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
builder.Services.AddFedExAddressValidation();

builder.Services.AddFedExRateProvider();

builder.Services.AddFedExShipmenProvider();

builder.Services.AddFedExTrackingProvider();

// configure json options
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.IncludeFields = true;
});

var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

(var sender, var receiver) = SetDefaultValues();

// address validation recieves a proposed address.
app.MapPost("/stamps/addressValidation", async (
    AddressValidationDto model,
    IStampsAddressValidationProvider addressProvider,
    CancellationToken cancellationToken) =>
{
    var address = new ValidateAddress(model.Id, model!.Address);
    var validatedAddress = await addressProvider.ValidateAddressAsync(address, cancellationToken);

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
    var address = new ValidateAddress(model.Id, model!.Address);
    var validatedAddress = await addressProvider.ValidateAddressAsync(address, cancellationToken);

    return Results.Json(validatedAddress, options);
})
.Accepts<ShipmentDto>("application/json")
.Produces<ValidateAddress>(StatusCodes.Status200OK, "application/json")
.WithName("FedExAddressValidation");

// getRates recieves a rate model containing destination address and package information.
app.MapPost("/stamps/getRates", async (
    ShipmentDto model,
    IStampsRateProvider rateProvider,
    CancellationToken cancellationToken) =>
{
    var listOfRates = new List<Rate>();

    var result = await GetShipmentRates(model, rateProvider, sender, receiver, null, cancellationToken);

    listOfRates.AddRange(result.SelectMany(x => x.Rates));

    return Results.Json(listOfRates, options);
})
.Accepts<ShipmentDto>("application/json")
.Produces<Shipment>(StatusCodes.Status200OK, "application/json")
.WithName("StampsGetRates");

app.MapPost("/fedex/getRates", async (
    ShipmentDto model,
    IFedExRateProvider rateProvider,
    CancellationToken cancellationToken) =>
{
    // create a package
    var package = new Package(
        model!.Package!.Length,
        model.Package.Width,
        model.Package.Height,
        model.Package.Weight,
        model.Package.InsuredValue,
        model.Package.SignatureRequiredOnDelivery);

    var defaultPackage = new Package(package.Dimensions, package.Weight, package.InsuredValue, package.SignatureRequiredOnDelivery);

    var config = new FedExRateConfigurator(model.Origin, model.Destination, defaultPackage);

    var result = new List<Rate>();

    foreach (var (shipment, serviceType) in config.Shipments)
    {
        var response = await rateProvider.GetRatesAsync(shipment, serviceType, cancellationToken);
        result.AddRange(response.Rates);
    }

    return Results.Json(result.OrderBy(x => x.TotalCharges), options);
})
.Accepts<ShipmentDto>("application/json")
.Produces<Shipment>(StatusCodes.Status200OK, "application/json")
.WithName("FedExGetRates");

// create the shipment when rates service type is selected.
app.MapPost("/stamps/createShipment", async (
    ShipmentDto model,
    string ServiceType,
    IStampsRateProvider rateProvider,
    IStampsShipmentProvider shipmentProvider,
    CancellationToken cancellationToken) =>
{
    var shipment = await GetShipmentRates(model, rateProvider, sender, receiver, StampsServiceType.FromName(ServiceType), cancellationToken);

    var shipmentRequestDetails = new ShipmentRequestDetails()
    {
        SelectedRate = shipment?.FirstOrDefault()?.Rates?.Where(x => x.Name == ServiceType)?.FirstOrDefault(),
    };

    // adds printed message
    shipmentRequestDetails.LabelOptions.Memo = "This will be orderId";

    var label = await shipmentProvider.CreateShipmentAsync(shipment.FirstOrDefault(), shipmentRequestDetails, cancellationToken);

    return Results.Json(label, options);
})
.Accepts<ShipmentDto>("application/json")
.Produces<ShipmentLabel>(StatusCodes.Status200OK, "application/json")
.WithName("StampsCreateShipment");

app.MapPost("/stamps/createInternationalShipment", async (
    InternationalShipmentDto model,
    string ServiceType,
    IStampsRateProvider rateProvider,
    IStampsShipmentProvider shipmentProvider,
    CancellationToken cancellationToken) =>
{
    var shipment = await GetShipmentRates(model, rateProvider, sender, receiver, StampsServiceType.FromName(ServiceType), cancellationToken);

    var shipmentRequestDetails = new ShipmentRequestDetails()
    {
        SelectedRate = shipment.FirstOrDefault().Rates.Where(x => x.Name == ServiceType).FirstOrDefault(),
        CustomsInformation = new CustomsInformation() { CustomsSigner = sender.FullName },
        DeclaredValue = model.Commodity.CustomsValue
    };

    // adds printed message
    shipmentRequestDetails.LabelOptions.Memo = "This will be orderId";

    shipment.FirstOrDefault().Commodities.Add(model.Commodity);

    var label = await shipmentProvider.CreateShipmentAsync(shipment.FirstOrDefault(), shipmentRequestDetails, cancellationToken);

    return Results.Json(label, options);
})
.Accepts<InternationalShipmentDto>("application/json")
.Produces<ShipmentLabel>(StatusCodes.Status200OK, "application/json")
.WithName("StampsCreateInternationalShipment");

// track shipment after it is created.
app.MapGet("/trackShipment/{id}", async (
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

(ContactInfo, ContactInfo) SetDefaultValues()
{
    var sender = new ContactInfo()
    {
        FirstName = "EasyKeys.com",
        LastName = "Fulfillment Center",
        Company = "EasyKeys.com",
        Email = "TestMe@EasyKeys.com",
        Department = "Software",
        PhoneNumber = "951-223-2222"
    };
    var receiver = new ContactInfo()
    {
        FirstName = "Customer",
        LastName = "Customer Last Name",
        Company = "BOFA",
        Email = "customer@email.com",
        Department = "IT Dept",
        PhoneNumber = "877.839.5397"
    };

    return (sender, receiver);
}

static async Task<List<Shipment>> GetShipmentRates(
    ShipmentDto model,
    IStampsRateProvider rateProvider,
    ContactInfo sender,
    ContactInfo receiver,
    StampsServiceType? serviceType,
    CancellationToken cancellationToken)
{
    // create a package
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
        sender,
        receiver,
        model.Package.ShipDate);

    var shipments = new List<Shipment>();

    foreach (var shipment in configurator.Shipments)
    {
        var result = await rateProvider.GetRatesAsync(shipment.shipment, shipment.rateOptions, cancellationToken);
        shipments.Add(result);
    }

    return shipments;
}
