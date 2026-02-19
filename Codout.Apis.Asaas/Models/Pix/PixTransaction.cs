using System;
using Codout.Apis.Asaas.Models.Pix.Enums;

namespace Codout.Apis.Asaas.Models.Pix
{
    public class PixTransaction
    {
        public string Id { get; set; }
        public string Payment { get; set; }
        public PixTransactionStatus Status { get; set; }
        public decimal Value { get; set; }
        public string Description { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? ScheduleDate { get; set; }
    }
}
