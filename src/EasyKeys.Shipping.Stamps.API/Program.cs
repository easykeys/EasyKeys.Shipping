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

using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

// set up config
var dic = new Dictionary<string, string>
    {
        { "AzureVault:BaseUrl", "https://easykeys.vault.azure.net/" },
    };

builder.Configuration.AddInMemoryCollection(dic);

builder.Configuration.AddAzureKeyVault(hostingEnviromentName: "Development", usePrefix: true);

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
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.IncludeFields = true;
});

var serviceProvider = builder.Services.BuildServiceProvider();

var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

(var sender, var receiver, var validatedAddress, var shipment, var label) = SetDefaultValues();

// address validation recieves a proposed address.
app.MapPost("/addressValidation", async (ValidateAddress address, CancellationToken cancellationToken) =>
{
    var addressProvider = serviceProvider.GetRequiredService<IStampsAddressValidationProvider>();

    validatedAddress = await addressProvider.ValidateAddressAsync(address, cancellationToken);

    return Results.Json(validatedAddress, options);
});

// getRates recieves a rate model containing destination address and package information.
app.MapPost("/getRates", async (RateModel rateModel, CancellationToken cancellationToken) =>
{
    var rateProvider = serviceProvider.GetRequiredService<IStampsRateProvider>();

    var package = new Package(Decimal.Parse(rateModel.Length), Decimal.Parse(rateModel.Width), Decimal.Parse(rateModel.Height), Decimal.Parse(rateModel.Weight), 20m);

    var config = new StampsRateConfigurator(rateModel.Origin, validatedAddress.ProposedAddress, package, sender, receiver);

    shipment = await rateProvider.GetRatesAsync(config.Shipments.FirstOrDefault().shipment, new RateRequestDetails(), cancellationToken);

    return Results.Json(shipment, options);
});

// create the shipment when rates service type is selected.
app.MapPost("/createShipment", async (string ServiceType, CancellationToken cancellationToken) =>
{
    var shipmentProvider = serviceProvider.GetRequiredService<IStampsShipmentProvider>();

    var shipmentRequestDetails = new ShipmentRequestDetails() { SelectedRate = shipment.Rates.Where(x => x.Name == ServiceType).FirstOrDefault() };

    label = await shipmentProvider.CreateShipmentAsync(shipment, shipmentRequestDetails, cancellationToken);

    return Results.Json(label, options);
});

// track shipment after it is created.
app.MapGet("/trackShipment", async (CancellationToken cancellationToken) =>
{
    var trackingInfo = await serviceProvider.GetRequiredService<IStampsTrackingProvider>()
                      .TrackShipmentAsync(label, cancellationToken);
    return Results.Json(trackingInfo, options);
});

app.Run();

(ContactInfo, ContactInfo, ValidateAddress, Shipment?, ShipmentLabel?) SetDefaultValues()
{
    var sender = new ContactInfo()
    {
        FirstName = "Brandon",
        LastName = "Moffett",
        Company = "EasyKeys.com",
        Email = "TestMe@EasyKeys.com",
        Department = "Software",
        PhoneNumber = "951-223-2222"
    };
    var receiver = new ContactInfo()
    {
        FirstName = "Fictitious Character",
        Company = "Marvel",
        Email = "FictitiousCharacter@marvel.com",
        Department = "SuperHero",
        PhoneNumber = "867-338-2737"
    };

    var validatedAddress = new ValidateAddress(Guid.NewGuid().ToString(), new Address(
                                            streetLine: "1550 central avenue",
                                            city: "riverside",
                                            stateOrProvince: "CA",
                                            postalCode: "92507",
                                            countryCode: "US"));

    var shipment = default(Shipment);

    var label = default(ShipmentLabel);

    return (sender, receiver, validatedAddress, shipment, label);
}
