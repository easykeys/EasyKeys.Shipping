﻿using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Abstractions.Services.Impl
{
    public class RatesService : IRatesService
    {
        private readonly IStampsClientService _stampsClient;

        public RatesService(IStampsClientService stampsClientService)
        {
            _stampsClient = stampsClientService;
        }

        public async Task<List<RateV40>> GetRatesResponseAsync(Shipment shipment, RateRequestDetails rateDetails, CancellationToken cancellationToken)
        {
            {
                var stampsClient = _stampsClient.CreateClient();

                var request = new GetRatesRequest()
                {
                    Item = await _stampsClient.GetTokenAsync(cancellationToken),

                    Rate = new RateV40()
                    {
                        From = new StampsClient.v111.Address()
                        {
                            FullName = shipment.SenderInformation.FullName,

                            FirstName = shipment.SenderInformation.FirstName,

                            LastName = shipment.SenderInformation.LastName,

                            Address1 = shipment.OriginAddress.StreetLine,

                            City = shipment.OriginAddress.City,

                            State = shipment.OriginAddress.StateOrProvince,

                            Country = shipment.OriginAddress.CountryCode,

                            ZIPCode = shipment.OriginAddress.PostalCode,

                            PhoneNumber = shipment.SenderInformation.PhoneNumber,

                            EmailAddress = shipment.RecipientInformation.Email
                        },

                        To = new StampsClient.v111.Address()
                        {
                            FullName = shipment.RecipientInformation.FullName,

                            FirstName = shipment.RecipientInformation.FirstName,

                            LastName = shipment.RecipientInformation.LastName,

                            Address1 = shipment.DestinationAddress.StreetLine,

                            City = shipment.DestinationAddress.City,

                            Province = shipment.DestinationAddress.CountryCode != "US" ? shipment.DestinationAddress.StateOrProvince : null,

                            State = shipment.DestinationAddress.CountryCode == "US" ? shipment.DestinationAddress.StateOrProvince : null,

                            Country = shipment.DestinationAddress.CountryCode,

                            PostalCode = shipment.DestinationAddress.CountryCode != "US" ? shipment.DestinationAddress.PostalCode : null,

                            ZIPCode = shipment.DestinationAddress.CountryCode == "US" ? shipment.DestinationAddress.PostalCode : null,

                            PhoneNumber = shipment.RecipientInformation.PhoneNumber,

                            EmailAddress = shipment.RecipientInformation.Email
                        },

                        Amount = 0.0m,

                        MaxAmount = 0.0m,

                        ServiceType = rateDetails.ServiceType switch
                        {
                            Models.ServiceType.USPS_PARCEL_SELECT_GROUND => StampsClient.v111.ServiceType.USPS,
                            Models.ServiceType.USPS_FIRST_CLASS_MAIL => StampsClient.v111.ServiceType.USFC,
                            Models.ServiceType.USPS_MEDIA_MAIL => StampsClient.v111.ServiceType.USMM,
                            Models.ServiceType.USPS_PRIORITY_MAIL => StampsClient.v111.ServiceType.USPM,
                            Models.ServiceType.USPS_PRIORITY_MAIL_EXPRESS => StampsClient.v111.ServiceType.USXM,
                            Models.ServiceType.USPS_PRIORITY_MAIL_EXPRESS_INTERNATIONAL => StampsClient.v111.ServiceType.USEMI,
                            Models.ServiceType.USPS_FIRST_CLASS_MAIL_INTERNATIONAL => StampsClient.v111.ServiceType.USFCI,
                            Models.ServiceType.USPS_PAY_ON_USE_RETURN => StampsClient.v111.ServiceType.USRETURN,
                            Models.ServiceType.USPS_LIBRARY_MAIL => StampsClient.v111.ServiceType.USLM,
                            Models.ServiceType.USPS_PRIORITY_MAIL_INTERNATIONAL => StampsClient.v111.ServiceType.USPMI,
                            _ => StampsClient.v111.ServiceType.Unknown
                        },

                        ServiceDescription = rateDetails.ServiceDescription,

                        DeliverDays = string.Empty,

                        ShipDate = shipment.Options.ShippingDate,

                        InsuredValue = rateDetails.InsuredValue,

                        RegisteredValue = rateDetails.RegisteredValue,

                        CODValue = rateDetails.CODValue,

                        DeclaredValue = rateDetails.DeclaredValue,

                        NonMachinable = rateDetails.NonMachinable,

                        RectangularShaped = rateDetails.RectangularShaped,

                        Prohibitions = rateDetails.Prohibitions,

                        Restrictions = rateDetails.Restrictions,

                        Observations = rateDetails.Observations,

                        Regulations = rateDetails.Regulations,

                        GEMNotes = rateDetails.GEMNotes,

                        MaxDimensions = rateDetails.MaxDimensions,

                        DimWeighting = rateDetails.DimWeighting,

                        EffectiveWeightInOunces = 0,

                        CubicPricing = rateDetails.CubicPricing,

                        ContentType = rateDetails.ContentType.ToLower() switch
                        {
                            "commercial_sample" => ContentTypeV2.CommercialSample,
                            "dangerous_goods" => ContentTypeV2.DangerousGoods,
                            "document" => ContentTypeV2.Document,
                            "gift" => ContentTypeV2.Gift,
                            "humanitarian" => ContentTypeV2.HumanitarianDonation,
                            "merchandise" => ContentTypeV2.Merchandise,
                            "returned_goods" => ContentTypeV2.ReturnedGoods,
                            _ => ContentTypeV2.Other
                        },

                        ContentTypeSpecified = rateDetails.ContentTypeSpecified,

                        // dont know what this is
                        EntryFacility = EntryFacilityV1.Unknown,

                        // dont know what this is
                        SortType = SortTypeV1.Unknown,
                    },
                    Carrier = rateDetails.Carrier.ToLower() switch
                    {
                        "usps" => Carrier.USPS,
                        "ups" => Carrier.UPS,
                        "dhlexpress" => Carrier.DHLExpress,
                        "fedex" => Carrier.FedEx,
                        _ => Carrier.USPS
                    }
                };

                request = ApplyPackageDetails(request, rateDetails, shipment);

                try
                {
                    var response = await stampsClient.GetRatesAsync(request);

                    response = ApplyAddOns(response, shipment);

                    return response.Rates.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        private GetRatesResponse ApplyAddOns(GetRatesResponse request, Shipment shipment)
        {
            var addOns = new List<AddOnV17>();

            foreach (var rate in request.Rates)
            {
                if (rate.RequiresAllOf != null)
                {
                    rate.RequiresAllOf.FirstOrDefault()
                        ?.AsEnumerable()
                        ?.ToList()
                        ?.ForEach(
                            x => addOns.Add(new AddOnV17() { AddOnType = x }));
                }

                addOns.Add(new AddOnV17() { AddOnDescription = "Tracking", AddOnType = AddOnTypeV17.USADC });

                if (shipment.DestinationAddress.CountryCode == "US")
                {
                    addOns.Add(new AddOnV17() { AddOnDescription = "Registered Mail", AddOnType = AddOnTypeV17.USAREG });
                }


                if (shipment.Packages.Any(x => x.SignatureRequiredOnDelivery))
                {
                    addOns.Add(new AddOnV17() { AddOnDescription = "Signature Confirmation", AddOnType = AddOnTypeV17.USASC });
                }

                rate.AddOns = addOns.ToArray();
            }

            return request;
        }

        private GetRatesRequest ApplyPackageDetails(GetRatesRequest request, RateRequestDetails rateDetails, Shipment shipment)
        {
            request.Rate.WeightLb = (double)shipment.Packages.Sum(x => x.Weight);

            request.Rate.WeightOz = 0.0;

            request.Rate.PackageType = rateDetails.PackageType switch
            {
                PackageType.Pak => PackageTypeV11.Pak,
                PackageType.Package => PackageTypeV11.Package,
                PackageType.Oversized_Package => PackageTypeV11.OversizedPackage,
                PackageType.Large_Package => PackageTypeV11.LargePackage,
                PackageType.PostCard => PackageTypeV11.Postcard,
                PackageType.Documents => PackageTypeV11.Documents,
                PackageType.Thick_Envelope => PackageTypeV11.ThickEnvelope,
                PackageType.Envelope => PackageTypeV11.Envelope,
                PackageType.Express_Envelope => PackageTypeV11.ExpressEnvelope,
                PackageType.Flat_Rate_Envelope => PackageTypeV11.FlatRateEnvelope,
                PackageType.Legal_Flat_Rate_Envelope => PackageTypeV11.LegalFlatRateEnvelope,
                PackageType.Unknown => PackageTypeV11.Unknown,
                PackageType.Letter => PackageTypeV11.Letter,
                PackageType.Large_Envelope_Or_Flat => PackageTypeV11.LargeEnvelopeorFlat,
                PackageType.Small_Flat_Rate_Box => PackageTypeV11.SmallFlatRateBox,
                PackageType.Flat_Rate_Box => PackageTypeV11.FlatRateBox,
                PackageType.Large_Flat_Rate_Box => PackageTypeV11.LargeFlatRateBox,
                PackageType.Flat_Rate_Padded_Envelope => PackageTypeV11.FlatRatePaddedEnvelope,
                PackageType.Regional_Rate_Box_A => PackageTypeV11.RegionalRateBoxA,
                PackageType.Regional_Rate_Box_B => PackageTypeV11.RegionalRateBoxB,
                PackageType.Regional_Rate_Box_C => PackageTypeV11.RegionalRateBoxC,
                _ => PackageTypeV11.Package
            };

            request.Rate.Length = (double)shipment.Packages.FirstOrDefault().Dimensions.Length;

            request.Rate.Width = (double)shipment.Packages.FirstOrDefault().Dimensions.Width;

            request.Rate.Height = (double)shipment.Packages.FirstOrDefault().Dimensions.Height;

            return request;
        }
    }
}