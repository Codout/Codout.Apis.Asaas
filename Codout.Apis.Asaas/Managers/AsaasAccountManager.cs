using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.AsaasAccount;
using Codout.Apis.Asaas.Models.Common.Base;

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

    public async Task<ResponseObject<Account>> Find(string accountId)
    {
        return await GetAsync<Account>(AsaasAccountRoute, accountId);
    }

    public async Task<ResponseObject<Account>> ResendActivationLink(string accountId)
    {
        var route = $"{AsaasAccountRoute}/{accountId}/resendActivationLink";
        return await PostAsync<Account>(route, new RequestParameters());
    }

    public async Task<ResponseObject<AccessToken>> CreateAccessToken(string accountId, CreateAccessTokenRequest requestObj)
    {
        var route = $"{AsaasAccountRoute}/{accountId}/accessTokens";
        return await PostAsync<AccessToken>(route, requestObj);
    }

    public async Task<ResponseList<AccessToken>> ListAccessTokens(string accountId, int offset, int limit)
    {
        var route = $"{AsaasAccountRoute}/{accountId}/accessTokens";
        return await GetListAsync<AccessToken>(route, offset, limit);
    }

    public async Task<ResponseObject<AccessToken>> UpdateAccessToken(string accountId, string accessTokenId, UpdateAccessTokenRequest requestObj)
    {
        var route = $"{AsaasAccountRoute}/{accountId}/accessTokens/{accessTokenId}";
        return await PutAsync<AccessToken>(route, requestObj);
    }

    public async Task<ResponseObject<BaseDeleted>> DeleteAccessToken(string accountId, string accessTokenId)
    {
        var route = $"{AsaasAccountRoute}/{accountId}/accessTokens/{accessTokenId}";
        return await DeleteAsync<BaseDeleted>(route);
    }
}
