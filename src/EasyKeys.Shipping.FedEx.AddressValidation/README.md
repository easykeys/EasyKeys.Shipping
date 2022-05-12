# EasyKeys.Shipping.FedEx.AddressValidation

[![Build status](https://ci.appveyor.com/api/projects/status/xp52rbpa9vmr1ck9?svg=true)](https://ci.appveyor.com/project/easykeys/easykeys-shipping)
[![NuGet](https://img.shields.io/nuget/v/EasyKeys.Shipping.FedEx.AddressValidation.svg)](https://www.nuget.org/packages?q=EasyKeys.Shipping.FedEx.AddressValidation)
![Nuget](https://img.shields.io/nuget/dt/EasyKeys.Shipping.FedEx.AddressValidation)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https://f.feedz.io/easykeys/core/shield/EasyKeys.Shipping.FedEx.AddressValidation/latest)](https://f.feedz.io/easykeys/core/packages/EasyKeys.Shipping.FedEx.AddressValidation/latest/download)

This library implements FedEx AV v4 for Address Validation.

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## Install

```bash
    dotnet add package EasyKeys.Shipping.FedEx.AddressValidation

## How FedEx Address Validation Works

The Address Validation Service performs the following tasks:

- As the first step, the service attempts to normalize the input 'address'. This can include replacing
  common roadway identifiers such as Street and Parkway with their standard abbreviations such
  as ST and PKWY, as well as reordering components of the address. If an input 'address' cannot
  be normalized, the EffectiveAddress returned will be the input 'address', with a State of RAW.
  Non address values are discarded. If needed by the user, they should be stored prior to
  submission. Refer to the attributes returned to help determine the problems with the address
  submitted.

- In the second step, the service attempts to standardize the normalized address, by finding a
  possible or actual address that is likely the one intended by the submitted 'address'. If that
standardization does not succeed, the EffectiveAddress returned will be the normalized form of
the input 'address', with a State of NORMALIZED. Refer to the attributes returned to help
determine the problems with the address submitted.

- Certain Attributes of that normalized 'address' will also be returned. If standardization does
succeed, the EffectiveAddress returned will be that real-world address, with a State of
STANDARDIZED. In this case, various additional Attributes of the standardized address and how
it was derived from the normalized address will be returned.

## Address Type

- RAW: The address as submitted in the request. 
  This is returned when that address could not be normalized or if the country is not supported.
- NORMALIZED: A formatted version of the address where elements are parsed and standard abbreviations are applied. 
  The Normalized address is returned when the Address Validation Service supports a country for address validation, 
  but cannot match the address against reference data. Reference data include postal data (and map data, for the US only).
- STANDARDIZED: A formatted and validated version of the address. 
  The standardized address is returned when the Address Validation Service can match the address against reference data. 
  Note that the Address Validation Service may make slight changes to the address in order to find a match. 
