



' retrieve values from azure vault

' Add services to the container.
' Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

' add stamps libraries

' add fedex libraries

' configure json options

' Configure the HTTP request pipeline.

' address validation recieves a proposed address.

' getRates recieves a rate model containing destination address and package information.
' create a package

' create the shipment when rates service type is selected.

' track shipment after it is created.

' cancel a label after it is created.
' create a package
Imports Minimal.Apis.Models

''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'var builder = Microsoft.Asp...' at character 590
''' 
''' 
''' Input:
''' 
var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'builder.Configuration.AddAz...' at character 709
''' 
''' 
''' Input:
''' 
''' // retrieve values from azure vault
''' builder.Configuration.AddAzureKeyVault(hostingEnviromentName: builder.Environment.EnvironmentName, usePrefix: true);
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'builder.Services.AddEndpoin...' at character 954
''' 
''' 
''' Input:
''' 
''' // Add services to the container.
''' // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
''' builder.Services.AddEndpointsApiExplorer();
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'builder.Services.AddSwagger...' at character 1001
''' 
''' 
''' Input:
''' 
''' builder.Services.AddSwaggerGen();
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'builder.Services.AddStampsA...' at character 1063
''' 
''' 
''' Input:
''' 
''' // add stamps libraries
''' builder.Services.AddStampsAddressProvider();
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'builder.Services.AddStampsR...' at character 1111
''' 
''' 
''' Input:
''' 
''' builder.Services.AddStampsRateProvider();
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'builder.Services.AddStampsS...' at character 1156
''' 
''' 
''' Input:
''' 
''' builder.Services.AddStampsShipmentProvider();
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'builder.Services.AddStampsT...' at character 1205
''' 
''' 
''' Input:
''' 
''' builder.Services.AddStampsTrackingProvider();
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'builder.Services.AddFedExAd...' at character 1278
''' 
''' 
''' Input:
''' 
''' // add fedex libraries
''' builder.Services.AddFedExAddressValidation();
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'builder.Services.AddFedExRa...' at character 1327
''' 
''' 
''' Input:
''' 
''' builder.Services.AddFedExRateProvider();
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'builder.Services.AddFedExSh...' at character 1371
''' 
''' 
''' Input:
''' 
''' builder.Services.AddFedExShipmenProvider();
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'builder.Services.AddFedExTr...' at character 1418
''' 
''' 
''' Input:
''' 
''' builder.Services.AddFedExTrackingProvider();
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'builder.Services.Configure<...' at character 1493
''' 
''' 
''' Input:
''' 
''' // configure json options
''' builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
''' {
'''     options.SerializerOptions.IncludeFields = true;
''' });
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'var options = new System.Te...' at character 1639
''' 
''' 
''' Input:
''' 
''' var options = new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web);
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'var app = builder.Build();' at character 1745
''' 
''' 
''' Input:
''' 
''' var app = builder.Build();
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'if (app.Environment.IsDevel...' at character 1816
''' 
''' 
''' Input:
''' 
''' // Configure the HTTP request pipeline.
''' if (app.Environment.IsDevelopment())
''' {
'''     app.UseSwagger();
'''     app.UseSwaggerUI();
''' }
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'app.UseHttpsRedirection();' at character 1910
''' 
''' 
''' Input:
''' 
''' app.UseHttpsRedirection();
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in '(var sender, var receiver) ...' at character 1940
''' 
''' 
''' Input:
''' 
''' (var sender, var receiver) = SetDefaultValues();
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'app.MapPost("/stamps/addres...' at character 2044
''' 
''' 
''' Input:
''' 
''' // address validation recieves a proposed address.
''' app.MapPost("/stamps/addressValidation", async (
'''     Minimal.Apis.Models.AddressValidationDto model,
'''     EasyKeys.Shipping.Stamps.AddressValidation.IStampsAddressValidationProvider addressProvider,
'''     System.Threading.CancellationToken cancellationToken) =>
''' {
'''     var address = new EasyKeys.Shipping.Abstractions.Models.ValidateAddress(model.Id, model!.Address);
'''     var validatedAddress = await addressProvider.ValidateAddressAsync(address, cancellationToken);
''' 
'''     return Microsoft.AspNetCore.Http.Results.Json(validatedAddress, options);
''' })
''' .Accepts<Minimal.Apis.Models.ShipmentDto>("application/json")
''' .Produces<EasyKeys.Shipping.Abstractions.Models.ValidateAddress>(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, "application/json")
''' .WithName("StampsAddressValidation");
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'app.MapPost("/fedex/address...' at character 2840
''' 
''' 
''' Input:
''' 
''' app.MapPost("/fedex/addressValidation", async (
'''     Minimal.Apis.Models.AddressValidationDto model,
'''     EasyKeys.Shipping.FedEx.AddressValidation.IFedExAddressValidationProvider addressProvider,
'''     System.Threading.CancellationToken cancellationToken) =>
''' {
'''     var address = new EasyKeys.Shipping.Abstractions.Models.ValidateAddress(model.Id, model!.Address);
'''     var validatedAddress = await addressProvider.ValidateAddressAsync(address, cancellationToken);
''' 
'''     return Microsoft.AspNetCore.Http.Results.Json(validatedAddress, options);
''' })
''' .Accepts<Minimal.Apis.Models.ShipmentDto>("application/json")
''' .Produces<EasyKeys.Shipping.Abstractions.Models.ValidateAddress>(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, "application/json")
''' .WithName("FedExAddressValidation");
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'app.MapPost("/stamps/getRat...' at character 3723
''' 
''' 
''' Input:
''' 
''' // getRates recieves a rate model containing destination address and package information.
''' app.MapPost("/stamps/getRates", async (
'''     Minimal.Apis.Models.ShipmentDto model,
'''     EasyKeys.Shipping.Stamps.Rates.IStampsRateProvider rateProvider,
'''     System.Threading.CancellationToken cancellationToken) =>
''' {
'''     var listOfRates = new System.Collections.Generic.List<EasyKeys.Shipping.Abstractions.Models.Rate>();

    var package = new EasyKeys.Shipping.Abstractions.Models.Package(
    model!.Package!.Length,
    model.Package.Width,
    model.Package.Height,
    model.Package.Weight,
    model.Package.InsuredValue,
    model.Package.SignatureRequiredOnDelivery);

    var configurator = new EasyKeys.Shipping.Stamps.Rates.StampsRateConfigurator(
    model.Origin!,
    model.Destination!,
    package,
    sender,
    receiver,
    model.Package.ShipDate);
''' 
'''     foreach (var shipment in configurator.Shipments)
'''     {
        var result = await rateProvider.GetRatesAsync(shipment.shipment, shipment.rateOptions, cancellationToken);
'''         listOfRates.AddRange(result.Rates);
'''     }
''' 
'''     return Microsoft.AspNetCore.Http.Results.Json(listOfRates.OrderBy(x => x.Name).ThenBy(x => x.TotalCharges), options);
''' })
''' .Accepts<Minimal.Apis.Models.ShipmentDto>("application/json")
''' .Produces<EasyKeys.Shipping.Abstractions.Models.Shipment>(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, "application/json")
''' .WithName("StampsGetRates");
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'app.MapPost("/fedex/getRate...' at character 5082
''' 
''' 
''' Input:
''' 
''' app.MapPost("/fedex/getRates", async (
'''     Minimal.Apis.Models.ShipmentDto model,
'''     EasyKeys.Shipping.FedEx.Rates.IFedExRateProvider rateProvider,
'''     System.Threading.CancellationToken cancellationToken) =>
''' {
'''     // create a package
'''     var package = new EasyKeys.Shipping.Abstractions.Models.Package(
'''         model!.Package!.Length,
'''         model.Package.Width,
'''         model.Package.Height,
'''         model.Package.Weight,
'''         model.Package.InsuredValue,
'''         model.Package.SignatureRequiredOnDelivery);
''' 
'''     var defaultPackage = new EasyKeys.Shipping.Abstractions.Models.Package(package.Dimensions, package.Weight, package.InsuredValue, package.SignatureRequiredOnDelivery);
''' 
'''     var config = new EasyKeys.Shipping.FedEx.Rates.FedExRateConfigurator(model.Origin, model.Destination, defaultPackage);
''' 
'''     var result = new System.Collections.Generic.List<EasyKeys.Shipping.Abstractions.Models.Rate>();
''' 
'''     foreach (var (shipment, serviceType) in config.Shipments)
'''     {
'''         var response = await rateProvider.GetRatesAsync(shipment, serviceType, cancellationToken);
'''         result.AddRange(response.Rates);
'''     }
''' 
'''     return Microsoft.AspNetCore.Http.Results.Json(result.OrderBy(x => x.TotalCharges), options);
''' })
''' .Accepts<Minimal.Apis.Models.ShipmentDto>("application/json")
''' .Produces<EasyKeys.Shipping.Abstractions.Models.Shipment>(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, "application/json")
''' .WithName("FedExGetRates");
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'app.MapPost("/stamps/create...' at character 6622
''' 
''' 
''' Input:
''' 
''' // create the shipment when rates service type is selected.
''' app.MapPost("/stamps/createShipment", async (
'''     Minimal.Apis.Models.ShipmentDto model,
'''     string ServiceType,
    string PackageTypeSelected,
    EasyKeys.Shipping.Stamps.Rates.IStampsRateProvider rateProvider,
'''     EasyKeys.Shipping.Stamps.Shipment.IStampsShipmentProvider shipmentProvider,
'''     System.Threading.CancellationToken cancellationToken) =>
''' {
    var package = new EasyKeys.Shipping.Abstractions.Models.Package(
        model!.Package!.Length,
        model.Package.Width,
        model.Package.Height,
        model.Package.Weight,
        model.Package.InsuredValue,
        model.Package.SignatureRequiredOnDelivery);

    var configurator = new EasyKeys.Shipping.Stamps.Rates.StampsRateConfigurator(
        model.Origin!,
        model.Destination!,
        package,
        sender,
        receiver,
        model.Package.ShipDate);

    var correctShipment = configurator.Shipments.Where(x => x.shipment.Options.PackagingType == PackageTypeSelected).FirstOrDefault();

    var shipmentRequestDetails = new EasyKeys.Shipping.Stamps.Shipment.Models.ShipmentRequestDetails();

    shipmentRequestDetails.LabelOptions.Memo = "This will be orderId";

    shipmentRequestDetails.RateRequestDetails.ServiceType = EasyKeys.Shipping.Stamps.Abstractions.Models.StampsServiceType.FromName(ServiceType);
''' 
'''     var label = await shipmentProvider.CreateShipmentAsync(correctShipment.shipment, shipmentRequestDetails, cancellationToken);
''' 
'''     return Microsoft.AspNetCore.Http.Results.Json(label, options);
''' })
''' .Accepts<Minimal.Apis.Models.ShipmentDto>("application/json")
''' .Produces<EasyKeys.Shipping.Abstractions.Models.ShipmentLabel>(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, "application/json")
''' .WithName("StampsCreateShipment");
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'app.MapPost("/stamps/create...' at character 8383
''' 
''' 
''' Input:
''' 
''' app.MapPost("/stamps/createInternationalShipment", async (
'''     Minimal.Apis.Models.InternationalShipmentDto model,
'''     string ServiceType,
    string PackageTypeSelected,
'''     EasyKeys.Shipping.Stamps.Rates.IStampsRateProvider rateProvider,
'''     EasyKeys.Shipping.Stamps.Shipment.IStampsShipmentProvider shipmentProvider,
'''     System.Threading.CancellationToken cancellationToken) =>
''' {
'''     var package = new EasyKeys.Shipping.Abstractions.Models.Package(
        model!.Package!.Length,
        model.Package.Width,
        model.Package.Height,
        model.Package.Weight,
        model.Package.InsuredValue,
        model.Package.SignatureRequiredOnDelivery);

    var configurator = new EasyKeys.Shipping.Stamps.Rates.StampsRateConfigurator(
        model.Origin!,
        model.Destination!,
        package,
        sender,
        receiver,
        model.Package.ShipDate);

    var correctShipment = configurator.Shipments.Where(x => x.shipment.Options.PackagingType == PackageTypeSelected).FirstOrDefault();

    var shipmentRequestDetails = new EasyKeys.Shipping.Stamps.Shipment.Models.ShipmentRequestDetails();

    shipmentRequestDetails.RateRequestDetails.ServiceType = EasyKeys.Shipping.Stamps.Abstractions.Models.StampsServiceType.FromName(ServiceType);
''' 
    shipmentRequestDetails.LabelOptions.Memo = "This will be orderId";

'''     correctShipment.shipment.Commodities.Add(model.Commodity);
''' 
'''     var label = await shipmentProvider.CreateShipmentAsync(correctShipment.shipment, shipmentRequestDetails, cancellationToken);
''' 
'''     return Microsoft.AspNetCore.Http.Results.Json(label, options);
''' })
''' .Accepts<Minimal.Apis.Models.InternationalShipmentDto>("application/json")
''' .Produces<EasyKeys.Shipping.Abstractions.Models.ShipmentLabel>(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, "application/json")
''' .WithName("StampsCreateInternationalShipment");
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'app.MapGet("/trackShipment/...' at character 10304
''' 
''' 
''' Input:
''' 
''' // track shipment after it is created.
''' app.MapGet("/trackShipment/{id}", async (
'''     string id,
'''     EasyKeys.Shipping.Stamps.Tracking.IStampsTrackingProvider trackingProvider,
'''     System.Threading.CancellationToken cancellationToken) =>
''' {
'''     var trackingInfo = await trackingProvider.TrackShipmentAsync(id, cancellationToken);
''' 
'''     return Microsoft.AspNetCore.Http.Results.Json(trackingInfo, options);
''' });
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'app.MapDelete("/stamps/canc...' at character 10723
''' 
''' 
''' Input:
''' 
''' // cancel a label after it is created.
''' app.MapDelete("/stamps/cancelShipment/{id}", async (
'''     string id,
'''     EasyKeys.Shipping.Stamps.Shipment.IStampsShipmentProvider shipmentProvider,
'''     System.Threading.CancellationToken cancellationToken) =>
''' {
'''     var result = await shipmentProvider.CancelShipmentAsync(id, cancellationToken);
''' 
'''     return Microsoft.AspNetCore.Http.Results.Json(result, options);
''' });
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'await app.RunAsync();' at character 11102
''' 
''' 
''' Input:
''' 
''' await app.RunAsync();
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in '(EasyKeys.Shipping.Abstract...' at character 11127
''' 
''' 
''' Input:
''' 
''' (EasyKeys.Shipping.Abstractions.Models.ContactInfo, EasyKeys.Shipping.Abstractions.Models.ContactInfo) SetDefaultValues()
''' {
'''     var sender = new EasyKeys.Shipping.Abstractions.Models.ContactInfo()
'''     {
'''         FirstName = "EasyKeys.com",
'''         LastName = "Fulfillment Center",
'''         Company = "EasyKeys.com",
'''         Email = "TestMe@EasyKeys.com",
'''         Department = "Software",
'''         PhoneNumber = "951-223-2222"
'''     };
'''     var receiver = new EasyKeys.Shipping.Abstractions.Models.ContactInfo()
'''     {
'''         FirstName = "Customer",
'''         LastName = "Customer Last Name",
'''         Company = "BOFA",
'''         Email = "customer@email.com",
'''         Department = "IT Dept",
'''         PhoneNumber = "877.839.5397"
'''     };
''' 
'''     return (sender, receiver);
''' }
''' 
''' 
''' Cannot convert GlobalStatementSyntax, CONVERSION ERROR: Conversion for GlobalStatement not implemented, please report this issue in 'static async System.Threadi...' at character 11910
''' 
''' 
''' Input:
''' 
''' static async System.Threading.Tasks.Task<System.Collections.Generic.List<EasyKeys.Shipping.Abstractions.Models.Shipment>> GetShipmentRates(
'''     Minimal.Apis.Models.ShipmentDto model,
'''     EasyKeys.Shipping.Stamps.Rates.IStampsRateProvider rateProvider,
'''     EasyKeys.Shipping.Abstractions.Models.ContactInfo sender,
'''     EasyKeys.Shipping.Abstractions.Models.ContactInfo receiver,
'''     EasyKeys.Shipping.Stamps.Abstractions.Models.StampsServiceType? serviceType,
'''     System.Threading.CancellationToken cancellationToken)
''' {
'''     // create a package
'''     var package = new EasyKeys.Shipping.Abstractions.Models.Package(
'''         model!.Package!.Length,
'''         model.Package.Width,
'''         model.Package.Height,
'''         model.Package.Weight,
'''         model.Package.InsuredValue,
'''         model.Package.SignatureRequiredOnDelivery);
''' 
'''     var configurator = new EasyKeys.Shipping.Stamps.Rates.StampsRateConfigurator(
'''         model.Origin!,
'''         model.Destination!,
'''         package,
'''         sender,
'''         receiver,
'''         model.Package.ShipDate);
''' 
'''     var shipments = new System.Collections.Generic.List<EasyKeys.Shipping.Abstractions.Models.Shipment>();

'''     foreach (var shipment in configurator.Shipments)
'''     {
        shipment.rateOptions.ServiceType = serviceType ?? EasyKeys.Shipping.Stamps.Abstractions.Models.StampsServiceType.Unknown;
'''         var result = await rateProvider.GetRatesAsync(shipment.shipment, shipment.rateOptions, cancellationToken);
'''         shipments.Add(result);
'''     }
''' 
'''     return shipments;
''' }
''' 
''' 