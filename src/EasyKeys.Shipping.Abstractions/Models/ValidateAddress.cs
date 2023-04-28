namespace EasyKeys.Shipping.Abstractions.Models;

/// <summary>
/// A class to be used for address validation purposes.
/// </summary>
public class ValidateAddress
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidateAddress"/> class.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="originalAddress"></param>
    public ValidateAddress(
        string id,
        Address originalAddress)
    {
        Id = id;
        OriginalAddress = originalAddress;
    }

    /// <summary>
    /// An unique id to identify the request to the address validation provider.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// If country is supported then returns true.
    /// </summary>
    public bool IsCountrySupported { get; set; }

    /// <summary>
    /// An original address supplied for the verification.
    /// </summary>
    public Address OriginalAddress { get; set; }

    /// <summary>
    /// A parsed address returned from the address verification provider.
    /// </summary>
    public Address? ProposedAddress { get; set; }

    /// <summary>
    ///     Errors returned by service provider (e.g. 'Wrong postal code').
    /// </summary>
    public List<Error> Errors { get; } = new List<Error>();

    /// <summary>
    ///     Internal library errors during interaction with service provider
    ///     (e.g. SoapException was thrown).
    /// </summary>
    public List<string> InternalErrors { get; } = new List<string>();

    public Dictionary<string, string> ValidationBag { get; } = new Dictionary<string, string>();
}
