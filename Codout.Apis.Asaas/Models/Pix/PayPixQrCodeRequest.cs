using System;

namespace Codout.Apis.Asaas.Models.Pix
{
    public class PayPixQrCodeRequest
    {
        public PayPixQrCodeInfo QrCode { get; set; }
        public decimal Value { get; set; }
        public string Description { get; set; }
        public DateTime? ScheduleDate { get; set; }
    }

    public class PayPixQrCodeInfo
    {
        public string Payload { get; set; }
    }
}
