using System.Collections.Generic;
using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Common;
using Codout.Apis.Asaas.Models.PaymentDunning;

namespace Codout.Apis.Asaas.Managers;

public class PaymentDunningManager(ApiSettings settings) : BaseManager(settings)
{
    private const string PaymentDunningRoute = "/paymentDunnings";

    public async Task<ResponseObject<PaymentDunning>> Create(CreatePaymentDunningRequest requestObj)
    {
        return await PostMultipartFormDataContentAsync<PaymentDunning>(PaymentDunningRoute, requestObj);
    }

    public async Task<ResponseObject<SimulatedPaymentDunning>> Simulate(SimulatePaymentDunningRequest requestObj)
    {
        // Schema oficial expoe "payment" como QUERY param, body vazio.
        // Mandar PaymentId no body funciona em alguns endpoints por leniencia,
        // mas o contrato documentado exige query string.
        var query = new RequestParameters();
        query.Add("payment", requestObj?.PaymentId);
        var route = $"{PaymentDunningRoute}/simulate{query.Build()}";

        return await PostAsync<SimulatedPaymentDunning>(route, new RequestParameters());
    }

    public async Task<ResponseObject<PaymentDunning>> Find(string paymentDunningId)
    {
        var route = $"{PaymentDunningRoute}/{paymentDunningId}";

        return await GetAsync<PaymentDunning>(route);
    }

    public async Task<ResponseList<PaymentDunning>> List(int offset, int limit, PaymentDunningListFilter filter = null)
    {
        var queryMap = new RequestParameters();
        if (filter != null) queryMap.AddRange(filter);

        return await GetListAsync<PaymentDunning>(PaymentDunningRoute, offset, limit, queryMap);
    }

    public async Task<ResponseList<PaymentDunningEventHistory>> ListEventHistory(string paymentDunningId, int offset, int limit)
    {
        var route = $"{PaymentDunningRoute}/{paymentDunningId}/history";

        return await GetListAsync<PaymentDunningEventHistory>(route, offset, limit);
    }

    public async Task<ResponseList<PaymentDunningPartialPayments>> ListPartialPaymentsReceived(string paymentDunningId, int offset, int limit)
    {
        var route = $"{PaymentDunningRoute}/{paymentDunningId}/partialPayments";

        return await GetListAsync<PaymentDunningPartialPayments>(route, offset, limit);
    }

    public async Task<ResponseList<PaymentDunningPaymentAvailable>> ListPaymentsAvailableForDunning(int offset, int limit)
    {
        var route = $"{PaymentDunningRoute}/paymentsAvailableForDunning";

        return await GetListAsync<PaymentDunningPaymentAvailable>(route, offset, limit);
    }

    public async Task<ResponseObject<PaymentDunning>> ResendDocument(string paymentDunningId, List<AsaasFile> asaasFiles)
    {
        var route = $"{PaymentDunningRoute}/{paymentDunningId}/documents";

        return await PostMultipartFormDataContentAsync<PaymentDunning>(route, new { documents = asaasFiles });
    }

    public async Task<ResponseObject<PaymentDunning>> Cancel(string paymentDunningId)
    {
        var route = $"{PaymentDunningRoute}/{paymentDunningId}/cancel";

        return await PostAsync<PaymentDunning>(route, new RequestParameters());
    }
}
