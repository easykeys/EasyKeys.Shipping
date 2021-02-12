# EasyKeys.Shipping.FedEx.AddressValidation

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
