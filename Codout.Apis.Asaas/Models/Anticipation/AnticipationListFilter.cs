using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Models.Anticipation.Enums;

namespace Codout.Apis.Asaas.Models.Anticipation
{
    public class AnticipationListFilter : RequestParameters
    {
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

        public AnticipationStatus? Status
        {
            get => Get<AnticipationStatus?>("status");
            set => Add("status", value);
        }
    }
}
