using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.CreditBureauReport;

namespace Codout.Apis.Asaas.Managers;

public class CreditBureauReportManager(ApiSettings settings) : BaseManager(settings)
{
    private const string CreditBureauReportRoute = "/creditBureauReport";

    public async Task<ResponseObject<CreditBureauReport>> Create(CreateCreditBureauReportRequest requestObj)
    {
        return await PostAsync<CreditBureauReport>(CreditBureauReportRoute, requestObj);
    }

    public async Task<ResponseList<CreditBureauReport>> List(int offset, int limit)
    {
        return await GetListAsync<CreditBureauReport>(CreditBureauReportRoute, offset, limit);
    }

    public async Task<ResponseObject<CreditBureauReport>> Find(string creditBureauReportId)
    {
        return await GetAsync<CreditBureauReport>(CreditBureauReportRoute, creditBureauReportId);
    }
}
