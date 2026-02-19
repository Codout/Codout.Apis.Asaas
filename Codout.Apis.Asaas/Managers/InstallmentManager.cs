using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Installment;
using Codout.Apis.Asaas.Models.Payment;

namespace Codout.Apis.Asaas.Managers;

public class InstallmentManager(ApiSettings settings) : BaseManager(settings)
{
    private const string InstallmentsRoute = "/installments";

    public async Task<ResponseObject<Installment>> Find(string installmentId)
    {
        var route = $"{InstallmentsRoute}/{installmentId}";
        return await GetAsync<Installment>(route);
    }

    public async Task<ResponseList<Installment>> List(int offset, int limit)
    {
        return await GetListAsync<Installment>(InstallmentsRoute, offset, limit);
    }

    public async Task<ResponseObject<DeletedInstallment>> Delete(string installmentId)
    {
        var route = $"{InstallmentsRoute}/{installmentId}";

        return await DeleteAsync<DeletedInstallment>(route);
    }

    public async Task<ResponseObject<Installment>> Refund(string installmentId)
    {
        var route = $"{InstallmentsRoute}/{installmentId}/refund";

        return await PostAsync<Installment>(route, new RequestParameters());
    }

    public async Task<ResponseList<Payment>> ListPaymentBook(string installmentId, int offset, int limit)
    {
        var route = $"{InstallmentsRoute}/{installmentId}/paymentBook";
        return await GetListAsync<Payment>(route, offset, limit);
    }
}
