using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Anticipation;

namespace Codout.Apis.Asaas.Managers;

public class AnticipationManager(ApiSettings settings) : BaseManager(settings)
{
    private const string AnticipationsRoute = "/anticipations";

    public async Task<ResponseObject<Anticipation>> Create(CreateAnticipationRequest requestObj)
    {
        return await PostMultipartFormDataContentAsync<Anticipation>(AnticipationsRoute, requestObj);
    }

    public async Task<ResponseObject<SimulatedAnticipation>> Simulate(SimulateAnticipationRequest requestObj)
    {
        var route = $"{AnticipationsRoute}/simulate";

        return await PostAsync<SimulatedAnticipation>(route, requestObj);
    }

    public async Task<ResponseObject<Anticipation>> Find(string anticipationId)
    {
        return await GetAsync<Anticipation>(AnticipationsRoute, anticipationId);
    }

    public async Task<ResponseList<Anticipation>> List(int offset, int limit, AnticipationListFilter filter = null)
    {
        var queryMap = new RequestParameters();
        if (filter != null) queryMap.AddRange(filter);

        var responseList = await GetListAsync<Anticipation>(AnticipationsRoute, offset, limit, queryMap);

        return responseList;
    }

    public async Task<ResponseObject<Anticipation>> SignAgreement(SignAnticipationAgreementRequest requestObj)
    {
        var route = $"{AnticipationsRoute}/agreement/sign";
        return await PostAsync<Anticipation>(route, requestObj);
    }
}
