using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Webhook;

namespace Codout.Apis.Asaas.Managers;

public class WebhookManager(ApiSettings settings) : BaseManager(settings)
{
    private const string WebhookRoute = "/webhook";

    public async Task<ResponseObject<Webhook>> CreateOrUpdatePaymentWebhook(WebhookRequest requestObj)
    {
        return await PostAsync<Webhook>(WebhookRoute, requestObj);
    }

    public async Task<ResponseObject<Webhook>> FindPaymentWebhook()
    {
        return await GetAsync<Webhook>(WebhookRoute);
    }

    public async Task<ResponseObject<Webhook>> CreateOrUpdateInvoiceWebhook(WebhookRequest requestObj)
    {
        var route = $"{WebhookRoute}/invoice";

        return await PostAsync<Webhook>(route, requestObj);
    }

    public async Task<ResponseObject<Webhook>> FindInvoiceWebhook()
    {
        var route = $"{WebhookRoute}/invoice";

        return await GetAsync<Webhook>(route);
    }

    public async Task<ResponseObject<Webhook>> CreateOrUpdateMobilePhoneRechargeWebhook(WebhookRequest requestObj)
    {
        var route = $"{WebhookRoute}/mobilePhoneRecharge";
        return await PostAsync<Webhook>(route, requestObj);
    }

    public async Task<ResponseObject<Webhook>> FindMobilePhoneRechargeWebhook()
    {
        var route = $"{WebhookRoute}/mobilePhoneRecharge";
        return await GetAsync<Webhook>(route);
    }
}
