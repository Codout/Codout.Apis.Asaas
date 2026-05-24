using System;
using Codout.Apis.Asaas.Core;

namespace Codout.Apis.Asaas.Models.CreditBureauReport;

public class CreditBureauReportListFilter : RequestParameters
{
    public DateTime? StartDate
    {
        get => Get<DateTime?>("startDate");
        set => Add("startDate", value);
    }

    public DateTime? EndDate
    {
        get => Get<DateTime?>("endDate");
        set => Add("endDate", value);
    }
}
