using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Codout.Apis.Asaas.Models.Common;
using Codout.Apis.Asaas.Models.Common.Enums;

namespace Codout.Apis.Asaas.Models.Payment
{
    public class CreatePaymentRequest
    {
        [JsonPropertyName("customer")]
        public string CustomerId { get; set; }

        public BillingType BillingType { get; set; }

        public decimal Value { get; set; }

        public DateTime? DueDate { get; set; }

        public string Description { get; set; }

        public string ExternalReference { get; set; }

        public int? InstallmentCount { get; set; }

        public decimal? InstallmentValue { get; set; }

        public decimal? TotalValue { get; set; }

        public Discount Discount { get; set; }

        public Interest Interest { get; set; }

        public Fine Fine { get; set; }

        public bool PostalService { get; set; }

        public CreditCardRequest CreditCard { get; set; }

        public CreditCardHolderInfoRequest CreditCardHolderInfo { get; set; }

        public string RemoteIp { get; set; }

        public List<Split> Split { get; set; } = [];

        public string? CreditCardToken { get; set; }
    }
}
