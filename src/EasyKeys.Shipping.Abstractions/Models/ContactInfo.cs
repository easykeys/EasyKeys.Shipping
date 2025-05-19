namespace EasyKeys.Shipping.Abstractions.Models;

public class ContactInfo
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    public string Company { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// taxId.
    /// </summary>
    public string TaxId { get; set; } = string.Empty;
}
