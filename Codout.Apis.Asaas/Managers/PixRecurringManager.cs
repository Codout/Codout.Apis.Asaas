using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.PixRecurring;

namespace Codout.Apis.Asaas.Managers;

public class PixRecurringManager(ApiSettings settings) : BaseManager(settings)
{
    private const string RecurringsRoute = "/pix/transactions/recurrings";

    public async Task<ResponseList<PixRecurringTransaction>> List(int offset, int limit, PixRecurringTransactionListFilter filter = null)
    {
        return await GetListAsync<PixRecurringTransaction>(RecurringsRoute, offset, limit, filter);
    }

    public async Task<ResponseObject<PixRecurringTransaction>> Find(string recurringId)
    {
        var route = $"{RecurringsRoute}/{recurringId}";
        return await GetAsync<PixRecurringTransaction>(route);
    }

    public async Task<ResponseObject<PixRecurringTransaction>> Cancel(string recurringId)
    {
        var route = $"{RecurringsRoute}/{recurringId}/cancel";
        return await PostAsync<PixRecurringTransaction>(route, new RequestParameters());
    }

    public async Task<ResponseObject<PixRecurringItemsResponse>> ListItems(string recurringId, int offset = 0, int limit = 10)
    {
        // API retorna envelope { data: [...] } sem hasMore/totalCount/limit/offset,
        // entao usamos GetAsync com query string manual e tipamos com wrapper proprio.
        var query = new RequestParameters
        {
            { "offset", offset },
            { "limit", limit }
        };
        var route = $"{RecurringsRoute}/{recurringId}/items{query.Build()}";
        return await GetAsync<PixRecurringItemsResponse>(route);
    }

    public async Task<ResponseObject<PixRecurringItem>> CancelItem(string itemId)
    {
        var route = $"{RecurringsRoute}/items/{itemId}/cancel";
        return await PostAsync<PixRecurringItem>(route, new RequestParameters());
    }
}
