# EasyKeys.Shipping.DHL.Rates

[![Build status](https://ci.appveyor.com/api/projects/status/xp52rbpa9vmr1ck9?svg=true)](https://ci.appveyor.com/project/easykeys/easykeys-shipping)
[![NuGet](https://img.shields.io/nuget/v/EasyKeys.Shipping.DHL.Rates.svg)](https://www.nuget.org/packages?q=EasyKeys.Shipping.DHL.Rates)
![Nuget](https://img.shields.io/nuget/dt/EasyKeys.Shipping.DHL.Rates)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https://f.feedz.io/easykeys/core/shield/EasyKeys.Shipping.DHL.Rates/latest)](https://f.feedz.io/easykeys/core/packages/EasyKeys.Shipping.DHL.Rates/latest/download)

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## Install
```bash
    dotnet add package EasyKeys.Shipping.DHL.Rates
```

# EasyKeys.Shipping.DHL - Rate Provider

This module provides **rate shopping support** via DHL Express API (v2). It enables shipment cost estimation and delivery date projection for international shipments using DHL Express services.

## Overview

The `DHLExpressRateProvider` class is responsible for:

- Fetching real-time rates using DHL’s `/rates` and `/rates-many` endpoints
- Supporting dimensional weight and insurance-based pricing
- Adding value-added services such as insurance and signature
- Populating estimated delivery dates and full price breakdowns

## Features

- ✅ Rate estimation with delivery time projection
- ✅ Supports multiple DHL products and services
- ✅ Value-added services (insurance, signature) auto-detected
- ✅ Uses two rate endpoints: `ExpApiRatesAsync` and `ExpApiRatesManyAsync`
- ✅ Implements `IDHLExpressRateProvider` abstraction
- ✅ Detailed error logging and fallbacks

## Registration

Add the provider to your DI container:

```csharp
services.AddDHLExpressRateProvider();
````

Make sure `DHLExpressApi` and `DHLExpressApiOptions` are configured and injected properly.

## Usage

```csharp
var shipmentWithRates = await rateProvider.GetRatesAsync(shipment);
```

Or, for richer pricing data:

```csharp
var shipmentWithRates = await rateProvider.GetRatesManyAsync(shipment);
```

After execution, `shipment.Rates` will be populated with estimated prices, delivery dates, and product codes.

## Example Rate Object

```csharp
new Rate(
    productCode: "P",
    productName: "Express Worldwide",
    currency: "USD",
    baseCharge: 42.00m,
    publishedCharge: 58.75m,
    estimatedDeliveryDate: DateTime.UtcNow.AddDays(2))
```

## Value-Added Services

The provider automatically includes additional services when detected in the shipment:

* `SF`: Signature on delivery
* `II`: Insurance (calculated based on max insured value)

These are added using DHL’s `ValueAddedServicesRates` payload structure.

## Error Handling

All exceptions and API errors are logged via `ILogger` and captured in `shipment.InternalErrors`.

## Design Approach

This class favors **purpose-driven implementation** over general abstraction. Future abstraction layers (e.g. for service filtering, pricing strategy, rate comparison) will be added **on demand**.

## References

* 📚 [DHL Express API v2 – Rates](https://developer.dhl.com/api-reference/dhl-express)
* 🧩 `EasyKeys.Shipping.Abstractions` – shared shipment/rate models
* 🛠️ `DHLExpressApi` – OpenAPI client for DHL Express

