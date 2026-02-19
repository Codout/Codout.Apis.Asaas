namespace Codout.Apis.Asaas.Models.Pix
{
    public class DecodedPixQrCode
    {
        public string Payload { get; set; }
        public string Type { get; set; }
        public string EndToEndIdentifier { get; set; }
        public decimal? OriginalValue { get; set; }
        public string ReceiverName { get; set; }
    }
}
