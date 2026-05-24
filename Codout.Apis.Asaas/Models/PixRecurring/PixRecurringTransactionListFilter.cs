using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Models.PixRecurring.Enums;

namespace Codout.Apis.Asaas.Models.PixRecurring;

public class PixRecurringTransactionListFilter : RequestParameters
{
    public PixRecurringStatus? Status
    {
        get => Get<PixRecurringStatus?>("status");
        set => Add("status", value);
    }

    public decimal? Value
    {
        get => Get<decimal?>("value");
        set => Add("value", value);
    }

    public string SearchText
    {
        get => this["searchText"];
        set => Add("searchText", value);
    }
}
