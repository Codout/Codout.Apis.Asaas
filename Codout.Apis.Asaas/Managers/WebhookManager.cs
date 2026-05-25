using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Common.Base;
using Codout.Apis.Asaas.Models.Webhook;

namespace Codout.Apis.Asaas.Managers;

public class WebhookManager(ApiSettings settings) : BaseManager(settings)
{
    private const string WebhooksRoute = "/webhooks";

    public async Task<ResponseObject<Webhook>> Create(CreateWebhookRequest requestObj)
    {
        return await PostAsync<Webhook>(WebhooksRoute, requestObj);
    }

    public async Task<ResponseList<Webhook>> List(int offset, int limit, WebhookListFilter filter = null)
    {
        var queryMap = new RequestParameters();
        if (filter != null) queryMap.AddRange(filter);

        return await GetListAsync<Webhook>(WebhooksRoute, offset, limit, queryMap);
    }

    public async Task<ResponseObject<Webhook>> Find(string webhookId)
    {
        var route = $"{WebhooksRoute}/{webhookId}";
        return await GetAsync<Webhook>(route);
    }

    public async Task<ResponseObject<Webhook>> Update(string webhookId, UpdateWebhookRequest requestObj)
    {
        var route = $"{WebhooksRoute}/{webhookId}";
        return await PutAsync<Webhook>(route, requestObj);
    }

    public async Task<ResponseObject<BaseDeleted>> Delete(string webhookId)
    {
        var route = $"{WebhooksRoute}/{webhookId}";
        return await DeleteAsync<BaseDeleted>(route);
    }

    public async Task<ResponseObject<Webhook>> RemoveBackoff(string webhookId)
    {
        var route = $"{WebhooksRoute}/{webhookId}/removeBackoff";
        return await PostAsync<Webhook>(route, new RequestParameters());
    }
}
