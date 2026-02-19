using System;

namespace Codout.Apis.Asaas.Models.PaymentDunning
{
    public class PaymentDunningPartialPayments
    {
        public decimal Value { get; set; }

        public string Description { get; set; }

        public DateTime PaymentDate { get; set; }
    }
}
