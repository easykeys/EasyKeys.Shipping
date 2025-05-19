# EasyKeys.Shipping.DHL.Shipment
[![Build status](https://ci.appveyor.com/api/projects/status/xp52rbpa9vmr1ck9?svg=true)](https://ci.appveyor.com/project/easykeys/easykeys-shipping)
[![NuGet](https://img.shields.io/nuget/v/EasyKeys.Shipping.DHL.Shipment.svg)](https://www.nuget.org/packages?q=EasyKeys.Shipping.DHL.Shipment)
![Nuget](https://img.shields.io/nuget/dt/EasyKeys.Shipping.DHL.Shipment)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https://f.feedz.io/easykeys/core/shield/EasyKeys.Shipping.DHL.Shipment/latest)](https://f.feedz.io/easykeys/core/packages/EasyKeys.Shipping.DHL.Shipment/latest/download)

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## Install

```bash
    dotnet add package EasyKeys.Shipping.DHL.Shipment
```

# EasyKeys.Shipping.DHL

This project provides DHL Express shipping support as part of the `EasyKeys.Shipping` library. It integrates with DHL’s Express REST API v2 and enables label creation, document handling, and rate estimation.

## Overview

The main implementation is found in the `DHLExpressShipmentProvider` class, which handles:

- Shipment creation
- Customs documentation
- Label generation
- Value-added services
- Error handling and logging

## Key Components

- `DHLExpressShipmentProvider` – orchestrates shipment creation.
- `DHLExpressApi` – low-level client to interact with DHL Express OpenAPI endpoints.
- `ShippingDetails` – model for shipment metadata (sender, recipient, commodities, etc).
- `SupermodelIoLogisticsExpress*` – DHL-specific models based on official OpenAPI specs.

## Features

- ✅ Supports commercial invoice, label, waybill generation
- ✅ Automatically enables paperless trade if eligible
- ✅ Includes value-added services (insurance, signature, paperless)
- ✅ Handles Incoterms, filing types, VAT registration
- ✅ Custom notification messages
- ✅ Detailed surcharges and shipment cost breakdown

## Quick Start

### 1. Configuration

Add your DHL credentials and settings to your `appsettings.json`:

```json
"DHLExpressApiOptions": {
  "AccountNumber": "YOUR_ACCOUNT",
  "ClientId": "YOUR_CLIENT_ID",
  "ClientSecret": "YOUR_CLIENT_SECRET",
  "ApiBaseUrl": "https://api-mock.dhl.com"
}
````

### 2. Register Services

In your `Startup.cs` or DI registration module:

```csharp
services.AddDHLExpressShipmentProvider();
```

### 3. Create a Shipment

```csharp
var label = await dhlProvider.CreateShipmentAsync(shipment, details);
```

The result includes base64-encoded label images, customs documents, and full cost breakdowns.

## Design Philosophy

This provider is purpose-built to handle DHL Express API integration with minimal abstraction overhead. **Abstraction will be added as needed** to support new use cases or additional DHL APIs such as pickup scheduling, rate quotes, and tracking.

The current design assumes:

* The DHL Express shipment API is the only DHL feature in use.
* Product codes and Incoterms are configured per shipment.
* Labels, invoices, and waybills are needed for each outbound package.

## Roadmap / Future Abstractions

Additional abstraction layers may be added as future requirements emerge, including:

* Tracking integration
* Pickup request services
* Rate shopping APIs
* Carrier-agnostic extensions of paperless trade logic

These will be introduced incrementally, avoiding premature complexity.

## References

* 📚 [DHL Express API v2 Documentation](https://developer.dhl.com/api-reference/dhl-express)
* 🧩 `EasyKeys.Shipping.Abstractions` for shared models/interfaces
