using System.Text.Json;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.AddressValidation;

using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add stamps libraries
builder.Services.AddStampsClient();
builder.Services.AddStampsAddressProvider();

// configure json options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.IncludeFields = true;
});

var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

// set up dev env & config
var dic = new Dictionary<string, string>
    {
        { "AzureVault:BaseUrl", "https://easykeys.vault.azure.net/" },
    };

var configBuilder = new ConfigurationBuilder().AddInMemoryCollection(dic);

configBuilder.AddAzureKeyVault(hostingEnviromentName: "Development", usePrefix: true);

builder.Services.AddSingleton<IConfiguration>(configBuilder.Build());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapPost("/addressValidation", async (ValidateAddress address, CancellationToken cancellationToken) =>
{
    var _addressProvider = builder.Services.BuildServiceProvider().GetRequiredService<IStampsAddressValidationProvider>();

    var result = await _addressProvider.ValidateAddressAsync(address, cancellationToken);

    return Results.Json(result, options);
});

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           Random.Shared.Next(-20, 55),
           summaries[Random.Shared.Next(summaries.Length)]
       ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
