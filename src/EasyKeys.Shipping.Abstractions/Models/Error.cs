namespace EasyKeys.Shipping.Abstractions.Models;

public class Error
{
    public string Number { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string HelpFile { get; set; } = string.Empty;

    public string HelpContext { get; set; } = string.Empty;
}
