using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Subscription.Enums;

namespace Codout.Apis.Asaas.Models.Subscription;

public class SubscriptionListFilter : RequestParameters
{
    public string CustomerId
    {
        get => this["customer"];
        set => Add("customer", value);
    }

    public string CustomerGroupName
    {
        get => this["customerGroupName"];
        set => Add("customerGroupName", value);
    }

    public BillingType? BillingType
    {
        get => Get<BillingType?>("billingType");
        set => Add("billingType", value);
    }

    public SubscriptionStatus? Status
    {
        get => Get<SubscriptionStatus?>("status");
        set => Add("status", value);
    }

    public bool? IncludeDeleted
    {
        get => Get<bool?>("includeDeleted");
        set => Add("includeDeleted", value);
    }

    public bool? DeletedOnly
    {
        get => Get<bool?>("deletedOnly");
        set => Add("deletedOnly", value);
    }

    public string ExternalReference
    {
        get => this["externalReference"];
        set => Add("externalReference", value);
    }

    public string Order
    {
        get => this["order"];
        set => Add("order", value);
    }

    public string Sort
    {
        get => this["sort"];
        set => Add("sort", value);
    }
}
