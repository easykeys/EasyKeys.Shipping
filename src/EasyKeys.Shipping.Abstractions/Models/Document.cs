namespace EasyKeys.Shipping.Abstractions.Models;

public class Document
{
    /// <summary>
    /// Number of printed copies required.
    /// </summary>
    public string CopiesToPrint { get; set; } = string.Empty;

    /// <summary>
    /// Name of document type.
    /// </summary>
    public string DocumentName { get; set; } = string.Empty;

    /// <summary>
    /// Type of the Image.
    /// </summary>
    public string ImageType { get; set; } = string.Empty;

    /// <summary>
    /// If multiple files.
    /// </summary>
    public List<byte[]>? Bytes { get; set; } = new List<byte[]>();
}
