using System.Globalization;

namespace EasyKeys.Shipping.DHL.Abstractions.Options;

public class DHLExpressApiOptions
{
    /// <summary>
    /// Please provide message reference.
    /// </summary>
    public string MessageReference { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Optional reference date in the  HTTP-date format https://tools.ietf.org/html/rfc7231#section-7.1.1.2.
    /// </summary>
    public string MessageReferenceDate { get; set; } = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

    /// <summary>
    /// Please provide name of the plugin (applicable to 3PV only).
    /// </summary>
    public string? PluginName { get; set; } 

    /// <summary>
    /// Please provide version of the plugin (applicable to 3PV only).
    /// </summary>
    public string? PluginVersion { get; set; }

    /// <summary>
    /// Please provide name of the shipping platform(applicable to 3PV only).
    /// </summary>
    public string? ShippingSystemPlatformName { get; set; }

    /// <summary>
    /// Please provide version of the shipping platform (applicable to 3PV only).
    /// </summary>
    public string? ShippingSystemPlatformVersion { get; set; }

    /// <summary>
    /// Please provide name of the webstore platform (applicable to 3PV only).
    /// </summary>
    public string? WebstorePlatformName { get; set; }

    /// <summary>
    /// Please provide version of the webstore platform (applicable to 3PV only).
    /// </summary>
    public string? WebstorePlatformVersion { get; set; }

    /// <summary>
    /// (Required) Interface version - do not change this field value.
    /// </summary>
    public string XVersion => "2.12.0";

    public string BaseUrl { get; set; } = "https://express.api.dhl.com/mydhlapi/test";

    public string ApiKey { get; set; } = string.Empty;

    public string ApiSecret { get; set; } = string.Empty;

    // export account number
    public string AccountNumber { get; set; } = string.Empty;

    // import account number
}
