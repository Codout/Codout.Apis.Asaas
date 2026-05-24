using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.FiscalInfo;

namespace Codout.Apis.Asaas.Managers;

public class FiscalInfoManager(ApiSettings settings) : BaseManager(settings)
{
    private const string FiscalInfoRoute = "/fiscalInfo";

    public async Task<ResponseObject<FiscalInfo>> CreateOrUpdate(CreateFiscalInfoRequest requestObj)
    {
        return await PostMultipartFormDataContentAsync<FiscalInfo>(FiscalInfoRoute, requestObj);
    }

    public async Task<ResponseObject<FiscalInfo>> Find()
    {
        return await GetAsync<FiscalInfo>(FiscalInfoRoute);
    }

    public async Task<ResponseList<MunicipalOption>> ListMunicipalOptions()
    {
        var route = $"{FiscalInfoRoute}/municipalOptions";
        return await GetListAsync<MunicipalOption>(route, 0, 100);
    }

    public async Task<ResponseList<MunicipalService>> ListServices(string description, int offset = 0, int limit = 10)
    {
        var queryMap = new RequestParameters
        {
            { "description", description }
        };

        var route = $"{FiscalInfoRoute}/services";

        return await GetListAsync<MunicipalService>(route, offset, limit, queryMap);
    }
}
