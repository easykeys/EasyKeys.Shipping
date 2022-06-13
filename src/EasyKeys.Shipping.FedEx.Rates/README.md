# EasyKeys.Shipping.FedEx.Rates

[![Build status](https://ci.appveyor.com/api/projects/status/xp52rbpa9vmr1ck9?svg=true)](https://ci.appveyor.com/project/easykeys/easykeys-shipping)
[![NuGet](https://img.shields.io/nuget/v/EasyKeys.Shipping.FedEx.Rates.svg)](https://www.nuget.org/packages?q=EasyKeys.Shipping.FedEx.Rates)
![Nuget](https://img.shields.io/nuget/dt/EasyKeys.Shipping.FedEx.Rates)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https://f.feedz.io/easykeys/core/shield/EasyKeys.Shipping.FedEx.Rates/latest)](https://f.feedz.io/easykeys/core/packages/EasyKeys.Shipping.FedEx.Rates/latest/download)

This library implements FedEx Rates v28.

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## Install

```bash
    dotnet add package EasyKeys.Shipping.FedEx.Rates
```

## Rate Service Details

The Rate and Rate Available Services operations provide a shipping rate quote for a specific service
combination depending on the origin and destination information supplied in the request.

The following details apply:

- Discount rates are available for all services and origin/destination pairs.
- FedEx list rates are available for FedEx Express, FedEx Ground, FedEx SmartPost and FedEx Freight services.
    When list rates are requested, both account specific rates and standard list rates are returned. \
    Note: List rates are not available for all FedEx Freight accounts.

- FedEx Freight shipper accounts can only request rates from origin at shipper address. FedEx Freight Bill To accounts can request rates from origins other than shipper address.

- Time in transit may be returned with the rates if it is specified in the request.

- The Rate operation returns the rate for the origin and destination by requested service. You will not receive service checking to validate whether that service is actually available for your ship date and origin/destination pair.

- The Rate Available Services operation returns the rate for the origin and destination for all available services. Note: Only valid services are returned.

- Rate and Rate Available Services for FedEx Express shipments support intra-Mexico shipping.

- Rating is available for FedEx SmartPost Shipping. See FedEx SmartPost Request Elements for more details. SmartPost outbound ship replies will also include rate elements with estimated rates. SmartPost Return shipping label replies will not include rate elements and estimates.
