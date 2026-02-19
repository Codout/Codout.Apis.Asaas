namespace Codout.Apis.Asaas.Models.Pix
{
    public class CreatePixStaticQrCodeRequest
    {
        public string AddressKey { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
    }
}
