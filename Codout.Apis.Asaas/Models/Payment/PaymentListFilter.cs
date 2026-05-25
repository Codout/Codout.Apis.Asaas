using System;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Invoice.Enums;
using Codout.Apis.Asaas.Models.Payment.Enums;

namespace Codout.Apis.Asaas.Models.Payment;

public class PaymentListFilter : RequestParameters
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

    public string SubscriptionId
    {
        get => this["subscription"];
        set => Add("subscription", value);
    }

    public string InstallmentId
    {
        get => this["installment"];
        set => Add("installment", value);
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

    public string ExternalReference
    {
        get => this["externalReference"];
        set => Add("externalReference", value);
    }

    public DateTime? PaymentDate
    {
        get => Get<DateTime?>("paymentDate");
        set => Add("paymentDate", value);
    }

    public InvoiceStatus? InvoiceStatus
    {
        get => Get<InvoiceStatus?>("invoiceStatus");
        set => Add("invoiceStatus", value);
    }

    public DateTime? EstimatedCreditDate
    {
        get => Get<DateTime?>("estimatedCreditDate");
        set => Add("estimatedCreditDate", value);
    }

    public string PixQrCodeId
    {
        get => this["pixQrCodeId"];
        set => Add("pixQrCodeId", value);
    }

    public bool? Anticipated
    {
        get => Get<bool?>("anticipated");
        set => Add("anticipated", value);
    }

    public bool? Anticipable
    {
        get => Get<bool?>("anticipable");
        set => Add("anticipable", value);
    }

    public string User
    {
        get => this["user"];
        set => Add("user", value);
    }

    public string CheckoutSession
    {
        get => this["checkoutSession"];
        set => Add("checkoutSession", value);
    }

    // Schema usa [ge]/[le] LOWERCASE para Payment (diferente de Invoice que usa [Ge]/[Le]).
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

    public DateTime? PaymentDateGE
    {
        get => Get<DateTime?>("paymentDate[ge]");
        set => Add("paymentDate[ge]", value);
    }

    public DateTime? PaymentDateLE
    {
        get => Get<DateTime?>("paymentDate[le]");
        set => Add("paymentDate[le]", value);
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
}
