using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Models.Common.Enums;

namespace Codout.Apis.Asaas.Models.Subscription
{
    public class SubscriptionListFilter : RequestParameters
    {
        public string CustomerId
        {
            get => this["customer"];
            set => Add("customer", value);
        }

        public BillingType? BillingType
        {
            get => Get<BillingType?>("billingType");
            set => Add("billingType", value);
        }

        public bool? IncludeDeleted
        {
            get => Get<bool?>("includeDeleted");
            set => Add("includeDeleted", value);
        }
    }
}
