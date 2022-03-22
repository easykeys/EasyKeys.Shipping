namespace EasyKeys.Shipping.Abstractions.Models
{
    public class Label
    {
        public string ImageType { get; set; } = String.Empty;

        public byte[]? Bytes { get; set; }

        /// <summary>
        ///     Internal library errors during interaction with service provider
        ///     (e.g. SoapException was thrown).
        /// </summary>
        public List<string> InternalErrors { get; } = new List<string>();
    }
}
