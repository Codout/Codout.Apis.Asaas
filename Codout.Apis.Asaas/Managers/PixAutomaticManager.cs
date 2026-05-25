using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Common.Base;
using Codout.Apis.Asaas.Models.PixAutomatic;

namespace Codout.Apis.Asaas.Managers;

public class PixAutomaticManager(ApiSettings settings) : BaseManager(settings)
{
    private const string AuthorizationsRoute = "/pix/automatic/authorizations";
    private const string PaymentInstructionsRoute = "/pix/automatic/paymentInstructions";

    public async Task<ResponseObject<PixAutomaticAuthorization>> CreateAuthorization(CreatePixAutomaticAuthorizationRequest requestObj)
    {
        return await PostAsync<PixAutomaticAuthorization>(AuthorizationsRoute, requestObj);
    }

    public async Task<ResponseList<PixAutomaticAuthorization>> ListAuthorizations(int offset, int limit, PixAutomaticAuthorizationListFilter filter = null)
    {
        var queryMap = new RequestParameters();
        if (filter != null) queryMap.AddRange(filter);

        return await GetListAsync<PixAutomaticAuthorization>(AuthorizationsRoute, offset, limit, queryMap);
    }

    public async Task<ResponseObject<PixAutomaticAuthorization>> FindAuthorization(string authorizationId)
    {
        var route = $"{AuthorizationsRoute}/{authorizationId}";
        return await GetAsync<PixAutomaticAuthorization>(route);
    }

    public async Task<ResponseObject<BaseDeleted>> CancelAuthorization(string authorizationId)
    {
        var route = $"{AuthorizationsRoute}/{authorizationId}";
        return await DeleteAsync<BaseDeleted>(route);
    }

    public async Task<ResponseObject<PixAutomaticPaymentInstruction>> FindPaymentInstruction(string paymentInstructionId)
    {
        var route = $"{PaymentInstructionsRoute}/{paymentInstructionId}";
        return await GetAsync<PixAutomaticPaymentInstruction>(route);
    }

    public async Task<ResponseList<PixAutomaticPaymentInstruction>> ListPaymentInstructions(int offset, int limit, PixAutomaticPaymentInstructionListFilter filter = null)
    {
        var queryMap = new RequestParameters();
        if (filter != null) queryMap.AddRange(filter);

        return await GetListAsync<PixAutomaticPaymentInstruction>(PaymentInstructionsRoute, offset, limit, queryMap);
    }
}
