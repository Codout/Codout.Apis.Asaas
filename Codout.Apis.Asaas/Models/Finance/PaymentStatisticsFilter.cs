using System;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Payment.Enums;

namespace Codout.Apis.Asaas.Models.Finance;

/// <summary>
/// Filtros para GET /v3/finance/payment/statistics.
/// Antes a chamada nao aceitava filtros — schema oficial expoe 11.
/// </summary>
public class PaymentStatisticsFilter : RequestParameters
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

    public PaymentStatus? Status
    {
        get => Get<PaymentStatus?>("status");
        set => Add("status", value);
    }

    public bool? Anticipated
    {
        get => Get<bool?>("anticipated");
        set => Add("anticipated", value);
    }

    public DateTime? DateCreatedGE
    {
        get => Get<DateTime?>("dateCreated[ge]");
        set => Add("dateCreated[ge]", value);
    }

    public DateTime? DateCreatedLE
    {
        get => Get<DateTime?>("dateCreated[le]");
        set => Add("dateCreated[le]", value);
    }

    public DateTime? DueDateGE
    {
        get => Get<DateTime?>("dueDate[ge]");
        set => Add("dueDate[ge]", value);
    }

    public DateTime? DueDateLE
    {
        get => Get<DateTime?>("dueDate[le]");
        set => Add("dueDate[le]", value);
    }

    public DateTime? EstimatedCreditDateGE
    {
        get => Get<DateTime?>("estimatedCreditDate[ge]");
        set => Add("estimatedCreditDate[ge]", value);
    }

    public DateTime? EstimatedCreditDateLE
    {
        get => Get<DateTime?>("estimatedCreditDate[le]");
        set => Add("estimatedCreditDate[le]", value);
    }

    public string ExternalReference
    {
        get => this["externalReference"];
        set => Add("externalReference", value);
    }
}
