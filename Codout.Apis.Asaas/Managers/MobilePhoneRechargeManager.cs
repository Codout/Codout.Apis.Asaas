using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.MobilePhoneRecharge;

namespace Codout.Apis.Asaas.Managers;

public class MobilePhoneRechargeManager(ApiSettings settings) : BaseManager(settings)
{
    private const string MobilePhoneRechargesRoute = "/mobilePhoneRecharges";

    public async Task<ResponseObject<MobilePhoneRecharge>> Create(CreateMobilePhoneRechargeRequest requestObj)
    {
        return await PostAsync<MobilePhoneRecharge>(MobilePhoneRechargesRoute, requestObj);
    }

    public async Task<ResponseList<MobilePhoneRecharge>> List(int offset, int limit)
    {
        return await GetListAsync<MobilePhoneRecharge>(MobilePhoneRechargesRoute, offset, limit);
    }

    public async Task<ResponseObject<MobilePhoneRecharge>> Find(string rechargeId)
    {
        return await GetAsync<MobilePhoneRecharge>(MobilePhoneRechargesRoute, rechargeId);
    }

    public async Task<ResponseObject<MobilePhoneRecharge>> Cancel(string rechargeId)
    {
        var route = $"{MobilePhoneRechargesRoute}/{rechargeId}/cancel";
        return await PostAsync<MobilePhoneRecharge>(route, new RequestParameters());
    }

    public async Task<ResponseObject<MobilePhoneProvider>> GetProvider(string phoneNumber)
    {
        var route = $"{MobilePhoneRechargesRoute}/{phoneNumber}/provider";
        return await GetAsync<MobilePhoneProvider>(route);
    }
}
