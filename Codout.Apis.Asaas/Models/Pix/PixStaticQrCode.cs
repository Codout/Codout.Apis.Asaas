using System;

namespace Codout.Apis.Asaas.Models.Pix
{
    public class PixStaticQrCode
    {
        public string Id { get; set; }
        public string EncodedImage { get; set; }
        public string Payload { get; set; }
        public bool AllowsMultiplePayments { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
