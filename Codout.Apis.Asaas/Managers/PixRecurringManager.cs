using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.PixRecurring;

namespace Codout.Apis.Asaas.Managers;

public class PixRecurringManager(ApiSettings settings) : BaseManager(settings)
{
    private const string RecurringsRoute = "/pix/transactions/recurrings";

    public async Task<ResponseList<PixRecurringTransaction>> List(int offset, int limit)
    {
        return await GetListAsync<PixRecurringTransaction>(RecurringsRoute, offset, limit);
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

    public async Task<ResponseList<PixRecurringItem>> ListItems(string recurringId, int offset, int limit)
    {
        var route = $"{RecurringsRoute}/{recurringId}/items";
        return await GetListAsync<PixRecurringItem>(route, offset, limit);
    }

    public async Task<ResponseObject<PixRecurringItem>> CancelItem(string itemId)
    {
        var route = $"{RecurringsRoute}/items/{itemId}/cancel";
        return await PostAsync<PixRecurringItem>(route, new RequestParameters());
    }
}
