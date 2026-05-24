using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Escrow;

namespace Codout.Apis.Asaas.Managers;

public class EscrowManager(ApiSettings settings) : BaseManager(settings)
{
    private const string AccountsRoute = "/accounts";
    private const string EscrowRoute = "/escrow";
    private const string PaymentsRoute = "/payments";

    public async Task<ResponseObject<EscrowConfig>> SaveSubaccountConfig(string accountId, SaveEscrowConfigRequest requestObj)
    {
        var route = $"{AccountsRoute}/{accountId}/escrow";
        return await PostAsync<EscrowConfig>(route, requestObj);
    }

    public async Task<ResponseObject<EscrowConfig>> GetSubaccountConfig(string accountId)
    {
        var route = $"{AccountsRoute}/{accountId}/escrow";
        return await GetAsync<EscrowConfig>(route);
    }

    public async Task<ResponseObject<EscrowConfig>> SaveDefaultConfig(SaveEscrowConfigRequest requestObj)
    {
        var route = $"{AccountsRoute}/escrow";
        return await PostAsync<EscrowConfig>(route, requestObj);
    }

    public async Task<ResponseObject<EscrowConfig>> GetDefaultConfig()
    {
        var route = $"{AccountsRoute}/escrow";
        return await GetAsync<EscrowConfig>(route);
    }

    public async Task<ResponseObject<Escrow>> FinishPaymentEscrow(string escrowId, FinishEscrowRequest requestObj = null)
    {
        var route = $"{EscrowRoute}/{escrowId}/finish";
        // Cast (object) eh necessario para que o operador ?? resolva para o overload
        // PostAsync<T>(string, object). Sem o cast, o compilador inferiria
        // FinishEscrowRequest como tipo comum e nao combinaria com RequestParameters.
        return await PostAsync<Escrow>(route, (object)requestObj ?? new RequestParameters());
    }

    public async Task<ResponseObject<Escrow>> GetPaymentEscrow(string paymentId)
    {
        var route = $"{PaymentsRoute}/{paymentId}/escrow";
        return await GetAsync<Escrow>(route);
    }
}
