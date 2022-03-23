namespace EasyKeys.Shipping.Abstractions.Models
{
    public class LabelDetails
    {
        public string ImageType { get; set; } = String.Empty;

        public List<byte[]> Bytes { get; set; } = new List<byte[]>();
    }
}
