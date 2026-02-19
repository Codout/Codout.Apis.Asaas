using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Wallet;

namespace Codout.Apis.Asaas.Managers;

public class WalletManager(ApiSettings settings) : BaseManager(settings)
{
    private const string WalletRoute = "/wallets";

    public async Task<ResponseList<Wallet>> List(int offset, int limit)
    {
        return await GetListAsync<Wallet>(WalletRoute, offset, limit);
    }
}
