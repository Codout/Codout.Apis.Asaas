using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Finance;

namespace Codout.Apis.Asaas.Managers;

public class FinanceManager(ApiSettings settings) : BaseManager(settings)
{
    private const string FinanceRoute = "/finance";
    private const string FinanceTransactionsRoute = "/financialTransactions";

    public async Task<ResponseObject<decimal>> Balance()
    {
        var route = $"{FinanceRoute}/balance";

        return await GetAsync<decimal>(route);
    }

    public async Task<ResponseList<FinancialTransaction>> ListTransactions(int offset, int limit, FinancialTransactionListFilter filter = null)
    {
        var queryMap = new RequestParameters();
        if (filter != null) queryMap.AddRange(filter);

        return await GetListAsync<FinancialTransaction>(FinanceTransactionsRoute, offset, limit, queryMap);
    }

    public async Task<ResponseObject<PaymentStatistics>> GetPaymentStatistics()
    {
        var route = $"{FinanceRoute}/payment/statistics";
        return await GetAsync<PaymentStatistics>(route);
    }

    public async Task<ResponseObject<SplitStatistics>> GetSplitStatistics()
    {
        var route = $"{FinanceRoute}/split/statistics";
        return await GetAsync<SplitStatistics>(route);
    }
}
