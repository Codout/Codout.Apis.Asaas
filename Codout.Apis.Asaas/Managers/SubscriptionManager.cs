using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Invoice;
using Codout.Apis.Asaas.Models.Payment;
using Codout.Apis.Asaas.Models.Subscription;

namespace Codout.Apis.Asaas.Managers;

public class SubscriptionManager(ApiSettings settings) : BaseManager(settings)
{
    private const string SubscriptionsRoute = "/subscriptions";

    #region Basic Resources

    public async Task<ResponseObject<Subscription>> Create(CreateSubscriptionRequest requestObj)
    {
        return await PostAsync<Subscription>(SubscriptionsRoute, requestObj);
    }

    public async Task<ResponseObject<Subscription>> Find(string subscriptionId)
    {
        var route = $"{SubscriptionsRoute}/{subscriptionId}";
        return await GetAsync<Subscription>(route);
    }

    public async Task<ResponseList<Subscription>> List(int offset, int limit, SubscriptionListFilter filter = null)
    {
        var queryMap = new RequestParameters();
        if (filter != null) queryMap.AddRange(filter);

        return await GetListAsync<Subscription>(SubscriptionsRoute, offset, limit, queryMap);
    }

    public async Task<ResponseObject<Subscription>> Update(string subscriptionId, UpdateSubscriptionRequest requestObj)
    {
        var route = $"{SubscriptionsRoute}/{subscriptionId}";
        return await PostAsync<Subscription>(route, requestObj);
    }

    public async Task<ResponseObject<DeletedSubscription>> Delete(string subscriptionId)
    {
        var route = $"{SubscriptionsRoute}/{subscriptionId}";

        return await DeleteAsync<DeletedSubscription>(route);
    }

    public async Task<ResponseList<Payment>> ListPayments(string subscriptionId, int offset, int limit)
    {
        var route = $"{SubscriptionsRoute}/{subscriptionId}/payments";
        return await GetListAsync<Payment>(route, offset, limit);
    }

    public async Task<ResponseList<Payment>> ListPaymentBook(string subscriptionId, int offset, int limit)
    {
        var route = $"{SubscriptionsRoute}/{subscriptionId}/paymentBook";
        return await GetListAsync<Payment>(route, offset, limit);
    }
    #endregion

    #region Invoice
    public async Task<ResponseList<Invoice>> ListInvoice(string subscriptionId, int offset, int limit, SubscriptionInvoiceListFilter filter = null)
    {
        var queryMap = new RequestParameters();
        if (filter != null) queryMap.AddRange(filter);

        var route = $"{SubscriptionsRoute}/{subscriptionId}/invoices";

        return await GetListAsync<Invoice>(route, offset, limit, queryMap);
    }

    public async Task<ResponseObject<SubscriptionInvoiceSettings>> CreateInvoiceSettings(string subscriptionId, CreateInvoiceSettingsRequest requestObj)
    {
        var route = $"{SubscriptionsRoute}/{subscriptionId}/invoiceSettings";

        return await PostAsync<SubscriptionInvoiceSettings>(route, requestObj);
    }

    public async Task<ResponseObject<SubscriptionInvoiceSettings>> UpdateInvoiceSettings(string subscriptionId, UpdateInvoiceSettingsRequest requestObj)
    {
        var route = $"{SubscriptionsRoute}/{subscriptionId}/invoiceSettings";
        return await PostAsync<SubscriptionInvoiceSettings>(route, requestObj);
    }

    public async Task<ResponseObject<SubscriptionInvoiceSettings>> FindInvoiceSettings(string subscriptionId)
    {
        var route = $"{SubscriptionsRoute}/{subscriptionId}/invoiceSettings";

        return await GetAsync<SubscriptionInvoiceSettings>(route);
    }

    public async Task<ResponseObject<DeletedInvoiceSettings>> DeleteInvoiceSettings(string subscriptionId)
    {
        var route = $"{SubscriptionsRoute}/{subscriptionId}/invoiceSettings";

        return await DeleteAsync<DeletedInvoiceSettings>(route);
    }
    #endregion
}
