using System;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Models.Invoice.Enums;

namespace Codout.Apis.Asaas.Models.Invoice
{
    public class InvoiceListFilter : RequestParameters
    {
        public DateTime? EffectiveDateGE
        {
            get => Get<DateTime?>("effectiveDate[ge]");
            set => Add("effectiveDate[ge]", value);
        }

        public DateTime? EffectiveDateLE
        {
            get => Get<DateTime?>("effectiveDate[le]");
            set => Add("effectiveDate[le]", value);
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

        public InvoiceStatus? Status
        {
            get => Get<InvoiceStatus?>("status");
            set => Add("status", value);
        }
    }
}
