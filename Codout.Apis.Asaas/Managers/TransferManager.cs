using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Transfer;
using Codout.Apis.Asaas.Models.Transfer.Base;

namespace Codout.Apis.Asaas.Managers;

public class TransferManager(ApiSettings settings) : BaseManager(settings)
{
    private const string TransfersRoute = "/transfers";

    public async Task<ResponseList<BaseTransfer>> List(int offset, int limit, TransferListFilter filter = null)
    {
        var queryMap = new RequestParameters();
        if (filter != null) queryMap.AddRange(filter);

        return await GetListAsync<BaseTransfer>(TransfersRoute, offset, limit, queryMap);
    }

    public async Task<ResponseObject<AsaasAccountTransfer>> Execute(AsaasAccountTransferRequest requestObj)
    {
        return await PostAsync<AsaasAccountTransfer>(TransfersRoute, requestObj);
    }

    public async Task<ResponseObject<BankAccountTransfer>> Execute(BankAccountTransferRequest requestObj)
    {
        return await PostAsync<BankAccountTransfer>(TransfersRoute, requestObj);
    }

    public async Task<ResponseObject<BaseTransfer>> Find(string transferId)
    {
        return await GetAsync<BaseTransfer>(TransfersRoute, transferId);
    }
}
