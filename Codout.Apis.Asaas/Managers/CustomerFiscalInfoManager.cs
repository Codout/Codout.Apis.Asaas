using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.CustomerFiscalInfo;

namespace Codout.Apis.Asaas.Managers;

public class CustomerFiscalInfoManager(ApiSettings settings) : BaseManager(settings)
{
    private const string CustomerFiscalInfoRoute = "/customerFiscalInfo";

    public async Task<ResponseObject<CustomerFiscalInfo>> CreateOrUpdate(CreateCustomerFiscalInfoRequest requestObj)
    {
        return await PostMultipartFormDataContentAsync<CustomerFiscalInfo>(CustomerFiscalInfoRoute, requestObj);
    }

    public async Task<ResponseObject<CustomerFiscalInfo>> Find()
    {
        return await GetAsync<CustomerFiscalInfo>(CustomerFiscalInfoRoute);
    }

    public async Task<ResponseList<MunicipalOption>> ListMunicipalOptions()
    {
        var route = $"{CustomerFiscalInfoRoute}/municipalOptions";
        return await GetListAsync<MunicipalOption>(route, 0, 100);
    }
}
