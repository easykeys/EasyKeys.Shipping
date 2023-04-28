namespace EasyKeys.Shipping.Stamps.Abstractions.Options;

public class StampsOptions
{
    /// <summary>
    /// Allows for configuration set correctly based on the enviroment.
    /// </summary>
    public bool IsDevelopment { get; set; }

    /// <summary>
    /// The base url for the SOAP calls.
    /// </summary>
    public string Url => IsDevelopment ? "https://swsim.testing.stamps.com/swsim/swsimv111.asmx" : "https://swsim.stamps.com/swsim/swsimv111.asmx";

    /// <summary>
    /// Stamps.com integration token.
    /// </summary>
    public string IntegrationId { get; set; } = string.Empty;

    /// <summary>
    /// Stamps.com username.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Stamps.com password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// In Kubernetes enviroments it is not possible to share the authetnication token,
    /// thus it can be disabled here.
    /// </summary>
    public bool UseAuthenticator { get; set; } = true;
}
