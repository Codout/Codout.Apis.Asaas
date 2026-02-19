using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Models.Invoice.Enums;

namespace Codout.Apis.Asaas.Models.Subscription
{
    public class SubscriptionInvoiceListFilter : RequestParameters
    {
        public InvoiceStatus? InvoiceStatus
        {
            get => Get<InvoiceStatus?>("status");
            set => Add("status", value);
        }
    }
}
