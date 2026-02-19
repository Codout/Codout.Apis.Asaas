using System;

namespace Codout.Apis.Asaas.Models.Pix
{
    public class PixPayment
    {
        public string Id { get; set; }
        public decimal Value { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public string TransactionReceiptUrl { get; set; }
    }
}
