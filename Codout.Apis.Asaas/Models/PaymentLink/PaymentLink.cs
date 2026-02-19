using System;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.PaymentLink.Enums;

namespace Codout.Apis.Asaas.Models.PaymentLink
{
    public class PaymentLink
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public decimal Value { get; set; }
        public bool Active { get; set; }
        public BillingType BillingType { get; set; }
        public ChargeType ChargeType { get; set; }
        public int DueDateLimitDays { get; set; }
        public string SubscriptionCycle { get; set; }
        public int MaxInstallmentCount { get; set; }
        public bool NotificationEnabled { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Deleted { get; set; }
    }
}
