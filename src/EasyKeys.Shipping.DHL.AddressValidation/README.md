# EasyKeys.Shipping.DHL.AddressValidation

[![Build status](https://ci.appveyor.com/api/projects/status/xp52rbpa9vmr1ck9?svg=true)](https://ci.appveyor.com/project/easykeys/easykeys-shipping)
[![NuGet](https://img.shields.io/nuget/v/EasyKeys.Shipping.DHL.AddressValidation.svg)](https://www.nuget.org/packages?q=EasyKeys.Shipping.DHL.AddressValidation)
![Nuget](https://img.shields.io/nuget/dt/EasyKeys.Shipping.DHL.AddressValidation)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https://f.feedz.io/easykeys/core/shield/EasyKeys.Shipping.DHL.AddressValidation/latest)](https://f.feedz.io/easykeys/core/packages/EasyKeys.Shipping.DHL.AddressValidation/latest/download)

This library implements DHL Express /address-validate API for address validation.

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!
Here's a complete `README.md` file tailored for the `DHLExpressAddressValidationProvider` class, ready for you to copy and paste:

# EasyKeys.Shipping.DHL - Address Validation

This component provides DHL Express **address validation support** within the `EasyKeys.Shipping` infrastructure. It uses the DHL Express API (v2) to validate destination addresses and propose corrections where applicable.

## Overview

The core implementation is provided by the `DHLExpressAddressValidationProvider` class. It implements both:

- `IDHLExpressAddressValidationProvider` – DHL-specific abstraction
- `IAddressValidationProvider` – shared interface used across providers

The validation relies on DHL’s `/address-validate` API endpoint and includes logging, error handling, and stopwatch-based performance diagnostics.

## Key Features

- ✅ Integration with DHL Express Address Validation API
- ✅ City + postal code resolution
- ✅ Graceful error handling and logging
- ✅ Stopwatch-based performance logging
- ✅ Implements generic `IAddressValidationProvider` for abstraction

## Example Usage

```csharp
services.AddDHLExpressAddressValidationProvider();
````

```csharp
var validated = await addressValidator.ValidateAddressAsync(new ValidateAddress
{
    OriginalAddress = new Address(
        streetLine: "123 Main St",
        streetLine2: "",
        city: "Los Angeles",
        stateOrProvince: "CA",
        postalCode: "90001",
        countryCode: "US")
});
```

After execution, `validated.ProposedAddress` may contain corrections such as a standardized postal code.

## Configuration

No configuration is required beyond DHL API client setup.

Ensure `DHLExpressApi` is registered and configured properly, including `ClientId`, `ClientSecret`, and `BaseUrl`.

## Design Notes

* **Error Logging:** All API exceptions are logged with details and added to `request.InternalErrors`.
* **Performance Tracking:** Uses `ValueStopwatch` to log execution duration in milliseconds.
* **Minimal Abstraction:** Only required behavior is implemented. Further abstraction will be introduced **as needed**.

## Output Behavior

* If the API returns warnings or no address info, the original address is retained.
* If a valid address is found, the `ProposedAddress` is populated using the DHL response.

## References

* 📚 [DHL Express Address-Validate API](https://developer.dhl.com/api-reference/dhl-express)
* 🧩 `EasyKeys.Shipping.Abstractions` – shared address and validation models
* 📦 `DHLExpressApi` – core client for OpenAPI-generated Express operations
