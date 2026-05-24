using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Checkout;

namespace Codout.Apis.Asaas.Managers;

public class CheckoutManager(ApiSettings settings) : BaseManager(settings)
{
    private const string CheckoutsRoute = "/checkouts";

    public async Task<ResponseObject<Checkout>> Create(CreateCheckoutRequest requestObj)
    {
        return await PostAsync<Checkout>(CheckoutsRoute, requestObj);
    }

    public async Task<ResponseObject<Checkout>> Cancel(string checkoutId)
    {
        var route = $"{CheckoutsRoute}/{checkoutId}/cancel";
        return await PostAsync<Checkout>(route, new RequestParameters());
    }
}
