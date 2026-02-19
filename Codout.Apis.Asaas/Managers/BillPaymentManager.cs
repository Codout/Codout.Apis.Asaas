using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Bill;

namespace Codout.Apis.Asaas.Managers;

public class BillPaymentManager(ApiSettings settings) : BaseManager(settings)
{
    private const string BillPaymentRoute = "/bill";

    public async Task<ResponseObject<BillPayment>> Create(CreateBillPaymentRequest requestObj)
    {
        return await PostAsync<BillPayment>(BillPaymentRoute, requestObj);
    }

    public async Task<ResponseObject<SimulatedBillPayment>> Simulate(SimulateBillPaymentRequest requestObj)
    {
        var simulateRoute = $"{BillPaymentRoute}/simulate";

        return await PostAsync<SimulatedBillPayment>(simulateRoute, requestObj);
    }

    public async Task<ResponseObject<BillPayment>> Find(string billPaymentId)
    {
        var route = $"{BillPaymentRoute}/{billPaymentId}";

        return await GetAsync<BillPayment>(route);
    }

    public async Task<ResponseList<BillPayment>> List(int offset, int limit)
    {
        return await GetListAsync<BillPayment>(BillPaymentRoute, offset, limit);
    }

    public async Task<ResponseObject<BillPayment>> Cancel(string billPaymentId)
    {
        var route = $"{BillPaymentRoute}/{billPaymentId}/cancel";

        return await PostAsync<BillPayment>(route, new RequestParameters());
    }
}
