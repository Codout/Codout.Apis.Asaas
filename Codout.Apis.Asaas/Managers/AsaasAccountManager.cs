using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.AsaasAccount;

namespace Codout.Apis.Asaas.Managers;

public class AsaasAccountManager(ApiSettings settings) : BaseManager(settings)
{
    private const string AsaasAccountRoute = "/accounts";

    public async Task<ResponseObject<Account>> Create(CreateAccountRequest requestObj)
    {
        return await PostAsync<Account>(AsaasAccountRoute, requestObj);
    }

    public async Task<ResponseList<Account>> List(int offset, int limit)
    {
        return await GetListAsync<Account>(AsaasAccountRoute, offset, limit);
    }
}
