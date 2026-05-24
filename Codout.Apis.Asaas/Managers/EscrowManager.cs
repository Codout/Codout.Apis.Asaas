using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Escrow;
using Codout.Apis.Asaas.Models.Payment;

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

    public async Task<ResponseObject<Payment>> FinishPaymentEscrow(string escrowId)
    {
        // A API documenta retorno PaymentGetResponseDTO (Payment) e request body
        // PaymentEscrowPathIdRequestDTO sem propriedades, entao mandamos {} vazio.
        var route = $"{EscrowRoute}/{escrowId}/finish";
        return await PostAsync<Payment>(route, new RequestParameters());
    }

    public async Task<ResponseObject<Escrow>> GetPaymentEscrow(string paymentId)
    {
        var route = $"{PaymentsRoute}/{paymentId}/escrow";
        return await GetAsync<Escrow>(route);
    }
}
