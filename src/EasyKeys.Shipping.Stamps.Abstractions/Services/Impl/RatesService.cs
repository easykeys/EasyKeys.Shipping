using EasyKeys.Shipping.Abstractions.Models;
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
                            FullName = shipment.SenderInfo.FullName,

                            FirstName = shipment.SenderInfo.FirstName,

                            LastName = shipment.SenderInfo.LastName,

                            Address1 = shipment.OriginAddress.StreetLine,

                            City = shipment.OriginAddress.City,

                            State = shipment.OriginAddress.StateOrProvince,

                            Country = shipment.OriginAddress.CountryCode,

                            ZIPCode = shipment.OriginAddress.PostalCode,

                            PhoneNumber = shipment.SenderInfo.PhoneNumber,

                            EmailAddress = shipment.RecipientInfo.Email
                        },

                        To = new StampsClient.v111.Address()
                        {
                            FullName = shipment.RecipientInfo.FullName,

                            FirstName = shipment.RecipientInfo.FirstName,

                            LastName = shipment.RecipientInfo.LastName,

                            Address1 = shipment.DestinationAddress.StreetLine,

                            City = shipment.DestinationAddress.City,

                            Province = shipment.DestinationAddress.CountryCode != "US" ? shipment.DestinationAddress.StateOrProvince : null,

                            State = shipment.DestinationAddress.CountryCode == "US" ? shipment.DestinationAddress.StateOrProvince : null,

                            Country = shipment.DestinationAddress.CountryCode,

                            PostalCode = shipment.DestinationAddress.CountryCode != "US" ? shipment.DestinationAddress.PostalCode : null,

                            ZIPCode = shipment.DestinationAddress.CountryCode == "US" ? shipment.DestinationAddress.PostalCode : null,

                            PhoneNumber = shipment.RecipientInfo.PhoneNumber,

                            EmailAddress = shipment.RecipientInfo.Email
                        },

                        Amount = rateDetails.Amount,

                        MaxAmount = rateDetails.MaxAmount,

                        ServiceType = rateDetails.ServiceType.Name switch
                        {
                            "USPS" => ServiceType.USPS,
                            "USFC" => ServiceType.USFC,
                            "USMM" => ServiceType.USMM,
                            "USPM" => ServiceType.USPM,
                            "USXM" => ServiceType.USXM,
                            "USEMI" => ServiceType.USEMI,
                            "USFCI" => ServiceType.USFCI,
                            "USRETURN" => ServiceType.USRETURN,
                            "USLM" => ServiceType.USLM,
                            "USPMI" => ServiceType.USPMI,
                            "Unkown" => ServiceType.Unknown,
                            _ => ServiceType.Unknown
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

                        ContentType = rateDetails.ContentType.Name switch
                        {
                            "COMMERCIAL_SAMPLE" => ContentTypeV2.CommercialSample,
                            "DANGEROUS_GOODS" => ContentTypeV2.DangerousGoods,
                            "DOCUMENT" => ContentTypeV2.Document,
                            "GIFT" => ContentTypeV2.Gift,
                            "HUMANITARIAN" => ContentTypeV2.HumanitarianDonation,
                            "MERCHANDISE" => ContentTypeV2.Merchandise,
                            "RETURNED_GOODS" => ContentTypeV2.ReturnedGoods,
                            _ => ContentTypeV2.Other
                        },

                        ContentTypeSpecified = rateDetails.ContentTypeSpecified,

                        // dont know what this is
                        EntryFacility = EntryFacilityV1.Unknown,

                        // dont know what this is
                        SortType = SortTypeV1.Unknown,
                    },
                    Carrier = rateDetails.Carrier.Name switch
                    {
                        "USPS" => Carrier.USPS,
                        "UPS" => Carrier.UPS,
                        "DHL_EXPRESS" => Carrier.DHLExpress,
                        "FEDEX" => Carrier.FedEx,
                        _ => Carrier.USPS
                    }
                };

                request = ApplyPackageDetails(request, rateDetails, shipment);

                var response = await stampsClient.GetRatesAsync(request);

                response = ApplyAddOns(response, shipment);

                return response.Rates.ToList();
            }
        }

        private GetRatesResponse ApplyAddOns(GetRatesResponse request, Shipment shipment)
        {
            foreach (var rate in request.Rates)
            {
                var addOns = new List<AddOnV17>();

                if (rate.RequiresAllOf != null)
                {
                    addOns.AddRange(AssignRequiredAddOnTypes(rate.RequiresAllOf));
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

        /// <summary>
        /// Must choose exactly one add-on from each set of add-ons listed in the
        /// requiresallof element in order to form a valid rate to be passed to CreateIndicium.
        /// The integration may use this hint in preparing a user interface with pre-validation for its users.
        /// <see href="file:///C:/Users/ucren/source/repos/EasyKeys.Shipping/src/EasyKeys.Shipping.Stamps.Abstractions/wsdls/SWS%20-%20Developer%20Guide%20v1.0.pdf">Documentta</see>.
        /// </summary>
        /// <param name="ArrayOfArrayOfTypes"></param>
        private List<AddOnV17> AssignRequiredAddOnTypes(AddOnTypeV17[][] ArrayOfArrayOfTypes)
        {
            var addOns = new List<AddOnV17>();
            for (var i = 0; i < ArrayOfArrayOfTypes.Length; i++)
            {
                addOns.Add(new AddOnV17() { AddOnType = ArrayOfArrayOfTypes[i].FirstOrDefault() });
            }

            return addOns;
        }

        private GetRatesRequest ApplyPackageDetails(GetRatesRequest request, RateRequestDetails rateDetails, Shipment shipment)
        {
            request.Rate.WeightLb = (double)shipment.Packages.FirstOrDefault().Weight;

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
