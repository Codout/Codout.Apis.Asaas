using System;
using Codout.Apis.Asaas.Models.Common;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Subscription.Enums;
using System.Text.Json.Serialization;

namespace Codout.Apis.Asaas.Models.Subscription
{
    public class CreateSubscriptionRequest {
        [JsonPropertyName("customer")]
        public string CustomerId { get; set; }

        public BillingType BillingType { get; set; }

        public decimal Value { get; set; }

        public DateTime NextDueDate { get; set; }

        public Discount Discount { get; set; }

        public Interest Interest { get; set; }

        public Fine Fine { get; set; }

        public Cycle? Cycle { get; set; }

        public string Description { get; set; }

        public DateTime? EndDate { get; set; }

        public int? MaxPayments { get; set; }

        public string ExternalReference { get; set; }

        public CreditCardRequest CreditCard { get; set; }

        public CreditCardHolderInfoRequest CreditCardHolderInfo { get; set; }

        public string CreditCardToken { get; set; }

        public string RemoteIp { get; set; }
    }
}
