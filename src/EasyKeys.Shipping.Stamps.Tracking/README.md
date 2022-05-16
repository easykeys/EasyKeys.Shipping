
# EasyKeys.Shipping.Stamps.Tracking
[![Build status](https://ci.appveyor.com/api/projects/status/xp52rbpa9vmr1ck9?svg=true)](https://ci.appveyor.com/project/easykeys/easykeys-shipping)
[![NuGet](https://img.shields.io/nuget/v/EasyKeys.Shipping.Stamps.Tracking.svg)](https://www.nuget.org/packages?q=EasyKeys.Shipping.Stamps.Tracking)
![Nuget](https://img.shields.io/nuget/dt/EasyKeys.Shipping.Stamps.Tracking)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https://f.feedz.io/easykeys/core/shield/EasyKeys.Shipping.Stamps.Tracking/latest)](https://f.feedz.io/easykeys/core/packages/EasyKeys.Shipping.Stamps.Tracking/latest/download)

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## Install

```bash
    dotnet add package EasyKeys.Shipping.Stamps.Tracking
```
## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## Install

```bash
    dotnet add package EasyKeys.Shipping.Stamps.Tracking
```
## Usage
```csharp

    builder.Services.AddStampsTrackingProvider();

    app.MapGet("/trackShipment/{id}", async (
        string id,
        IStampsTrackingProvider trackingProvider,
        CancellationToken cancellationToken) =>
    {

        var trackingInfo = await trackingProvider.TrackShipmentAsync(id, cancellationToken);

        return Results.Json(trackingInfo, options);
    });
```

## Tracking Provider Details

Returns a list of tracking events using the shipment's tracking number.


