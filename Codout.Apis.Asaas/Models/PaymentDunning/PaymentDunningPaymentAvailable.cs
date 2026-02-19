using System;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Payment.Enums;
using System.Text.Json.Serialization;

namespace Codout.Apis.Asaas.Models.PaymentDunning
{
    public class PaymentDunningPaymentAvailable
    {
        [JsonPropertyName("payment")]
        public string PaymentId { get; set; }

        [JsonPropertyName("customer")]
        public string CustomerId { get; set; }

        public decimal Value { get; set; }

        public PaymentStatus Status { get; set; }

        public BillingType BillingType { get; set; }

        public DateTime DueDate { get; set; }

        public PaymentDunningTypeSimulations TypeSimulations { get; set; }
    }
}
