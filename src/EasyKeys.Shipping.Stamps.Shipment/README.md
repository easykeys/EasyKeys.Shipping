
# EasyKeys.Shipping.Stamps.Shipment
[![Build status](https://ci.appveyor.com/api/projects/status/xp52rbpa9vmr1ck9?svg=true)](https://ci.appveyor.com/project/easykeys/easykeys-shipping)
[![NuGet](https://img.shields.io/nuget/v/EasyKeys.Shipping.Stamps.Shipment.svg)](https://www.nuget.org/packages?q=EasyKeys.Shipping.Stamps.Shipment)
![Nuget](https://img.shields.io/nuget/dt/EasyKeys.Shipping.Stamps.Shipment)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https://f.feedz.io/easykeys/core/shield/EasyKeys.Shipping.Stamps.Shipment/latest)](https://f.feedz.io/easykeys/core/packages/EasyKeys.Shipping.Stamps.Shipment/latest/download)

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## Install

```bash
    dotnet add package EasyKeys.Shipping.Stamps.Shipment
```
## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## Install

```bash
    dotnet add package EasyKeys.Shipping.Stamps.Shipment
```

## Usage
```csharp

    builder.Services.AddStampsShipmentProvider();

    // create shipment
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

        var label = await shipmentProvider.CreateShipmentAsync(shipment.FirstOrDefault(), shipmentRequestDetails, cancellationToken);

        return Results.Json(label, options);
    })

    // create international shipment 
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

        shipment.FirstOrDefault().Commodities.Add(model.Commodity);

        var label = await shipmentProvider.CreateShipmentAsync(shipment.FirstOrDefault(), shipmentRequestDetails, cancellationToken);

        return Results.Json(label, options);
    }) 

    // cancel shipment
    app.MapGet("/cancelShipment/{id}", async (
        string id,
        IStampsTrackingProvider trackingProvider,
        CancellationToken cancellationToken) =>
    {

        var result = await trackingProvider.CancelShipmentAsync(id, cancellationToken);

        return Results.Json(result, options);
    });

    public class ShipmentDto
    {
        public Address? Origin { get; set; }

        public Address? Destination { get; set; }

        public PackageDto? Package { get; set; }
    }

    public class InternationalShipmentDto : ShipmentDto
    {
        public Commodity? Commodity { get; set; }
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
            serviceType,
            model.Package.ShipDate);

        var shipments = new List<Shipment>();

        foreach (var shipment in configurator.Shipments)
        {
            var result = await rateProvider.GetRatesAsync(shipment.shipment, shipment.rateOptions, cancellationToken);
            shipments.Add(result);
        }

        return shipments;
    }

```

## Shipment Provider Details

### Create Shipment
1. Configures a Create Shipment request using the ShipmentRequestDetails class.
2. Configures a RateRequest using the service type selected found in the ShipmentRequestDetails class.
3. Consumes a single rate from the rate request.
4. Makes a Create Shipment request using the rate object provided and returns a generated shipment label.

### Cancel Shipment
Cancels a shipment based on the tracking number.

## Additional Information

This service is dependent on the internal RatesService class.
