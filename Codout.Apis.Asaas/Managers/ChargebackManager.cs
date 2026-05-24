using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Chargeback;

namespace Codout.Apis.Asaas.Managers;

public class ChargebackManager(ApiSettings settings) : BaseManager(settings)
{
    private const string ChargebacksRoute = "/chargebacks";
    private const string PaymentsRoute = "/payments";

    public async Task<ResponseList<Chargeback>> List(int offset, int limit)
    {
        return await GetListAsync<Chargeback>(ChargebacksRoute, offset, limit);
    }

    public async Task<ResponseObject<Chargeback>> FindByPayment(string paymentId)
    {
        var route = $"{PaymentsRoute}/{paymentId}/chargeback";
        return await GetAsync<Chargeback>(route);
    }

    public async Task<ResponseObject<Chargeback>> CreateDispute(string chargebackId, CreateChargebackDisputeRequest requestObj)
    {
        var route = $"{ChargebacksRoute}/{chargebackId}/dispute";
        return await PostMultipartFormDataContentAsync<Chargeback>(route, requestObj);
    }
}
