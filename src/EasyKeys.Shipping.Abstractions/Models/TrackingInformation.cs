namespace EasyKeys.Shipping.Abstractions.Models;

public class TrackingInformation
{
    public List<TrackingEvent> TrackingEvents { get; set; } = new List<TrackingEvent>();

    /// <summary>
    ///     Errors returned by service provider (e.g. 'Wrong postal code').
    /// </summary>
    public IList<Error> Errors { get; } = new List<Error>();

    /// <summary>
    ///     Internal library errors during interaction with service provider
    ///     (e.g. SoapException was thrown).
    /// </summary>
    public IList<string> InternalErrors { get; } = new List<string>();
}
