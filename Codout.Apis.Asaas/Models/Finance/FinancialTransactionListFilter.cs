using System;
using Codout.Apis.Asaas.Core;

namespace Codout.Apis.Asaas.Models.Finance
{
    public class FinancialTransactionListFilter : RequestParameters
    {
        public DateTime? StartDate
        {
            get => Get<DateTime?>("startDate");
            set => Add("startDate", value);
        }

        public DateTime? FinishDate
        {
            get => Get<DateTime?>("finishDate");
            set => Add("finishDate", value);
        }
    }
}
