using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.MyAccount;

namespace Codout.Apis.Asaas.Managers;

public class MyAccountManager(ApiSettings settings) : BaseManager(settings)
{
    private const string MyAccountRoute = "/myAccount";
    private const string PaymentCheckoutConfigRoute = MyAccountRoute + "/paymentCheckoutConfig";
    private const string FeesRoute = MyAccountRoute + "/fees";
    private const string AccountNumberRoute = MyAccountRoute + "/accountNumber";

    public async Task<ResponseObject<MyAccount>> Find()
    {
        return await GetAsync<MyAccount>(MyAccountRoute);
    }

    public async Task<ResponseObject<PaymentCheckoutConfig>> CreatePaymentCheckoutConfig(CreatePaymentCheckoutConfigRequest requestObj)
    {
        return await PostMultipartFormDataContentAsync<PaymentCheckoutConfig>(PaymentCheckoutConfigRoute, requestObj);
    }

    public async Task<ResponseObject<PaymentCheckoutConfig>> FindPaymentCheckoutConfig()
    {
        return await GetAsync<PaymentCheckoutConfig>(PaymentCheckoutConfigRoute);
    }

    public async Task<ResponseObject<Fees>> FindFees()
    {
        return await GetAsync<Fees>(FeesRoute);
    }

    public async Task<ResponseObject<AccountNumber>> FindAccountNumber()
    {
        return await GetAsync<AccountNumber>(AccountNumberRoute);
    }
}