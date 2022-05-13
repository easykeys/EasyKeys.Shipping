using System.Text.Json;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.AddressValidation;
using EasyKeys.Shipping.Stamps.API.Models;
using EasyKeys.Shipping.Stamps.Rates;
using EasyKeys.Shipping.Stamps.Shipment;
using EasyKeys.Shipping.Stamps.Shipment.DependencyInjection;
using EasyKeys.Shipping.Stamps.Shipment.Models;
using EasyKeys.Shipping.Stamps.Tracking;
using EasyKeys.Shipping.Stamps.Tracking.DependencyInjection;

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
app.MapPost("/addressValidation", async (
    AddressValidationDto model,
    IStampsAddressValidationProvider addressProvider,
    CancellationToken cancellationToken) =>
{
    var address = new ValidateAddress(model.Id, model!.Address);
    var validatedAddress = await addressProvider.ValidateAddressAsync(address, cancellationToken);

    return Results.Json(validatedAddress, options);
});

// getRates recieves a rate model containing destination address and package information.
app.MapPost("/getRates", async (
    ShipmentDto model,
    IStampsRateProvider rateProvider,
    CancellationToken cancellationToken) =>
{
    var result = await GetShipmentRates(model, rateProvider, sender, receiver, cancellationToken);

    return Results.Json(result, options);
});

// create the shipment when rates service type is selected.
app.MapPost("/createShipment", async (
    ShipmentDto model,
    string ServiceType,
    IStampsRateProvider rateProvider,
    IStampsShipmentProvider shipmentProvider,
    CancellationToken cancellationToken) =>
{
    var shipment = await GetShipmentRates(model, rateProvider, sender, receiver, cancellationToken);

    var shipmentRequestDetails = new ShipmentRequestDetails()
    {
        SelectedRate = shipment.Rates.Where(x => x.Name == ServiceType).FirstOrDefault()
    };

    var label = await shipmentProvider.CreateShipmentAsync(shipment, shipmentRequestDetails, cancellationToken);

    await File.WriteAllBytesAsync("label.png", label.Labels[0].Bytes[0]);

    return Results.Json(label, options);
});

// track shipment after it is created.
app.MapGet("/trackShipment/{id}", async (
    string id,
    IStampsTrackingProvider trackingProvider,
    CancellationToken cancellationToken) =>
{
    var labelInfo = new ShipmentLabel();

    labelInfo.Labels.Add(new PackageLabelDetails() { TrackingId = id });

    var trackingInfo = await trackingProvider.TrackShipmentAsync(labelInfo, cancellationToken);

    return Results.Json(trackingInfo, options);
});

// cancel a label after it is created.
app.MapDelete("/cancelShipment/{id}", async (
    string id,
    IStampsShipmentProvider shipmentProvider,
    CancellationToken cancellationToken) =>
{
    var labelInfo = new ShipmentLabel();

    labelInfo.Labels.Add(new PackageLabelDetails() { TrackingId = id });

    var trackingInfo = await shipmentProvider.CancelShipmentAsync(labelInfo, cancellationToken);

    return Results.Json(trackingInfo, options);
});

app.Run();

(ContactInfo, ContactInfo) SetDefaultValues()
{
    var sender = new ContactInfo()
    {
        FirstName = "EasyKeys.com",
        LastName = "Fulfilment Center",
        Company = "EasyKeys.com",
        Email = "TestMe@EasyKeys.com",
        Department = "Software",
        PhoneNumber = "951-223-2222"
    };
    var receiver = new ContactInfo()
    {
        FirstName = "Customer @ EasyKeys.com",
        Company = "BOFA",
        Email = "customer@email.com",
        Department = "IT Dept",
        PhoneNumber = "877.839.5397"
    };

    return (sender, receiver);
}

static async Task<Shipment> GetShipmentRates(ShipmentDto model, IStampsRateProvider rateProvider, ContactInfo sender, ContactInfo receiver, CancellationToken cancellationToken)
{
    // create a package
    var package = new Package(
        model!.Package!.Length,
        model.Package.Width,
        model.Package.Height,
        model.Package.Weight,
        model.Package.InsuredValue,
        model.Package.SignatureRequiredOnDelivery);

    var configurator = new StampsRateConfigurator(model!.Origin, model!.Destination, package, sender, receiver, model.Package.ShipDate);

    var (shipment, rateOptions) = configurator.Shipments.FirstOrDefault();

    // todo: fix the issue with raterequest details
    var result = await rateProvider.GetRatesAsync(shipment, new RateRequestDetails(), cancellationToken);
    return result;
}
