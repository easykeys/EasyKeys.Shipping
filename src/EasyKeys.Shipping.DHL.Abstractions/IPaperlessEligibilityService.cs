namespace EasyKeys.Shipping.DHL.Abstractions;

public interface IPaperlessEligibilityService
{
    bool IsPaperlessAvailable(string countryCode, double value);
}
