using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Invoice;

namespace Codout.Apis.Asaas.Managers;

public class InvoiceManager(ApiSettings settings) : BaseManager(settings)
{
    private const string InvoicesRoute = "/invoices";

    public async Task<ResponseObject<Invoice>> Schedule(CreateInvoiceRequest requestObj)
    {
        return await PostAsync<Invoice>(InvoicesRoute, requestObj);
    }

    public async Task<ResponseObject<Invoice>> Update(string invoiceId, UpdateInvoiceRequest requestObj)
    {
        var route = $"{InvoicesRoute}/{invoiceId}";
        return await PutAsync<Invoice>(route, requestObj);
    }

    public async Task<ResponseObject<Invoice>> Find(string invoiceId)
    {
        var route = $"{InvoicesRoute}/{invoiceId}";
        return await GetAsync<Invoice>(route);
    }

    public async Task<ResponseList<Invoice>> List(int offset, int limit, InvoiceListFilter filter = null)
    {
        var queryMap = new RequestParameters();
        if (filter != null) queryMap.AddRange(filter);

        return await GetListAsync<Invoice>(InvoicesRoute, offset, limit, queryMap);
    }

    public async Task<ResponseObject<Invoice>> Authorize(string invoiceId)
    {
        var route = $"{InvoicesRoute}/{invoiceId}/authorize";

        return await PostAsync<Invoice>(route, new RequestParameters());
    }

    public async Task<ResponseObject<Invoice>> Cancel(string invoiceId)
    {
        var route = $"{InvoicesRoute}/{invoiceId}/cancel";
        return await PostAsync<Invoice>(route, new RequestParameters());
    }

    public async Task<ResponseList<MunicipalService>> ListMunicipalServices(string serviceDescription)
    {
        var queryMap = new RequestParameters
        {
            { "description", serviceDescription }
        };

        var route = $"{InvoicesRoute}/municipalServices";

        return await GetListAsync<MunicipalService>(route, 0, 0, queryMap);
    }
}
