# EasyKeys.Shipping.Stamps.AddressValidation


This library implements Stamps.com SWSIM v111 for Address cleanse & validation

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## Install

```bash
    dotnet add package EasyKeys.Shipping.Stamps.AddressValidation

## How Stamps.com Address Validation Works

CleanseAddress as the name suggests provides address verification functionality.  

## Domestic Address Validation

SWS/IM requires you to verify the address for all domestic shipments9
. Once you have the destination
address call CleanseAddress, this will verify the address is valid and also make formatting changes to
meet USPS addressing standard.  
When the response is received from the request the AddressMatch and CityStateZipOk fields are checked. 

__AddressMatch__ : This is a Boolean value that is True if an exact match was found. 

__CityStateZipOk__ : If this is true that means the street address provided
could not be verified but the city, state and the zip match each other. In this case, you can continue to
print postage for this address but it is probably worth informing the user that the street address could
not be verified. 

Both fields are recorded in the ValidationBag of the ValidateAddress object.
  
##  International Address Validation   
                                                     
Addresses cannot be verified for international shipments but we do verify the destination country. Please ensure
that you use a country name from the accepted list published here ‐ http://pe.usps.com/text/imm/immctry.htm.

## Address Type

- RAW: The address as submitted in the request. 
  This is returned when that address could not be normalized or if the country is not supported.
- NORMALIZED: A formatted version of the address where elements are parsed and standard abbreviations are applied. 
  The Normalized address is returned when the Address Validation Service supports a country for address validation, 
  but cannot match the address against reference data. Reference data include postal data (and map data, for the US only).
- STANDARDIZED: A formatted and validated version of the address. 
  The standardized address is returned when the Address Validation Service can match the address against reference data. 
  Note that the Address Validation Service may make slight changes to the address in order to find a match. 
