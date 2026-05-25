using System;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Models.Invoice.Enums;

namespace Codout.Apis.Asaas.Models.Invoice;

public class InvoiceListFilter : RequestParameters
{
    // Schema oficial usa "effectiveDate[Ge]" e "[Le]" com G/L maiusculos,
    // diferente do padrao snake-case usado em outros filtros do Asaas.
    public DateTime? EffectiveDateGE
    {
        get => Get<DateTime?>("effectiveDate[Ge]");
        set => Add("effectiveDate[Ge]", value);
    }

    public DateTime? EffectiveDateLE
    {
        get => Get<DateTime?>("effectiveDate[Le]");
        set => Add("effectiveDate[Le]", value);
    }

    public string PaymentId
    {
        get => this["payment"];
        set => Add("payment", value);
    }

    public string InstallmentId
    {
        get => this["installment"];
        set => Add("installment", value);
    }

    public string CustomerId
    {
        get => this["customer"];
        set => Add("customer", value);
    }

    public string ExternalReference
    {
        get => this["externalReference"];
        set => Add("externalReference", value);
    }

    public InvoiceStatus? Status
    {
        get => Get<InvoiceStatus?>("status");
        set => Add("status", value);
    }
}
