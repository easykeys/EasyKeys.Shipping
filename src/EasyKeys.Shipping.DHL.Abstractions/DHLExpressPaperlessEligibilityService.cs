using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;

using EasyKeys.Shipping.DHL.Abstractions.Models;

using Microsoft.Extensions.Logging;

namespace EasyKeys.Shipping.DHL.Abstractions;

public class DHLExpressPaperlessEligibilityService : IPaperlessEligibilityService
{
    private readonly ConcurrentDictionary<string, CountryPaperlessInfo> _paperlessData;
    private readonly ILogger<DHLExpressPaperlessEligibilityService> _logger;

    public DHLExpressPaperlessEligibilityService(ILogger<DHLExpressPaperlessEligibilityService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        // Get path to the current .exe
        var exePath = Assembly.GetExecutingAssembly().Location;
        var exeDirectory = Path.GetDirectoryName(exePath);

        var dataFileName = "Data/paperless_countries.json";
        var dataFilePath = Path.Combine(exeDirectory!, dataFileName);

        if (!File.Exists(dataFilePath))
        {
            throw new FileNotFoundException("Paperless country data not found.", dataFilePath);
        }

        var json = File.ReadAllText(dataFilePath);

        var countries = JsonSerializer.Deserialize<List<CountryPaperlessInfo>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<CountryPaperlessInfo>();

        _paperlessData = new ConcurrentDictionary<string, CountryPaperlessInfo>(
            countries.ToDictionary(x => x.CountryCode.ToUpperInvariant(), x => x));
    }

    public bool IsPaperlessAvailable(string countryCode, double value)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                return false;
            }

            if (!_paperlessData.TryGetValue(countryCode.ToUpperInvariant(), out var info))
            {
                return false;
            }

            if (!info.PaperlessAvailable)
            {
                return false;
            }

            return !info.ValueLimit.HasValue || value <= info.ValueLimit.Value;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error checking paperless eligibility for country {CountryCode} with value {Value}", countryCode, value);
            return false;
        }
    }
}
