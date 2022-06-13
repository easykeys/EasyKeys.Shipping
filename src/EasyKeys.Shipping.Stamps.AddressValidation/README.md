# EasyKeys.Shipping.Stamps.AddressValidation

[![Build status](https://ci.appveyor.com/api/projects/status/xp52rbpa9vmr1ck9?svg=true)](https://ci.appveyor.com/project/easykeys/easykeys-shipping)
[![NuGet](https://img.shields.io/nuget/v/EasyKeys.Shipping.Stamps.AddressValidation.svg)](https://www.nuget.org/packages?q=EasyKeys.Shipping.Stamps.AddressValidation)
![Nuget](https://img.shields.io/nuget/dt/EasyKeys.Shipping.Stamps.AddressValidation)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https://f.feedz.io/easykeys/core/shield/EasyKeys.Shipping.Stamps.AddressValidation/latest)](https://f.feedz.io/easykeys/core/packages/EasyKeys.Shipping.Stamps.AddressValidation/latest/download)

This library implements Stamps.com SWSIM v111 for Address cleanse & validation

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## Install

```bash
    dotnet add package EasyKeys.Shipping.Stamps.AddressValidation
```

## Usage

```csharp

    builder.Services.AddStampsAddressProvider();

    app.MapPost("/addressValidation", async (
        AddressValidationDto model,
        IStampsAddressValidationProvider addressProvider,
        CancellationToken cancellationToken) =>
    {
        var address = new ValidateAddress(model.Id, model!.Address);
        var validatedAddress = await addressProvider.ValidateAddressAsync(address, cancellationToken);

        return Results.Json(validatedAddress, options);
    });

    public class AddressValidationDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public Address? Address { get; set; }
    }
```
## How Stamps.com Address Validation Works

CleanseAddress as the name suggests provides address verification functionality.

## Domestic Address Validation

SWS/IM requires you to verify the address for all domestic shipments.
Once you have the destination address call CleanseAddress, this will verify the address is valid and also make formatting changes to
meet USPS addressing standard.
When the response is received from the request the AddressMatch and `CityStateZipOk` fields are checked.

__AddressMatch__ : This is a Boolean value that is True if an exact match was found.

__CityStateZipOk__ : If this is true that means the street address provided
could not be verified but the city, state and the zip match each other. In this case, you can continue to
print postage for this address but it is probably worth informing the user that the street address could
not be verified.

Both fields are recorded in the ValidationBag of the ValidateAddress object.

## International Address Validation

Addresses cannot be verified for international shipments but we do verify the destination country. Please ensure
that you use a country name from the accepted list published here ‐ http://pe.usps.com/text/imm/immctry.htm.

## Additional Information

Please note that in order to recieve a successful response <b>sender & recipient contact information must be provided</b>.
