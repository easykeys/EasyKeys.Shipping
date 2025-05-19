# EasyKeys.Shipping.DHL.Abstractions

[![Build status](https://ci.appveyor.com/api/projects/status/xp52rbpa9vmr1ck9?svg=true)](https://ci.appveyor.com/project/easykeys/easykeys-shipping)
[![NuGet](https://img.shields.io/nuget/v/EasyKeys.Shipping.DHL.Abstractions.svg)](https://www.nuget.org/packages?q=EasyKeys.Shipping.DHL.Abstractions)
![Nuget](https://img.shields.io/nuget/dt/EasyKeys.Shipping.DHL.Abstractions)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https://f.feedz.io/easykeys/core/shield/EasyKeys.Shipping.DHL.Abstractions/latest)](https://f.feedz.io/easykeys/core/packages/EasyKeys.Shipping.DHL.Abstractions/latest/download)

This library provides abstractions for DHL options, models, and OpenApi implementation.

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## Install

```bash
    dotnet add package EasyKeys.Shipping.DHL.Abstractions
```

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

```csharp
    services.AddDHLExpressClient();
```
