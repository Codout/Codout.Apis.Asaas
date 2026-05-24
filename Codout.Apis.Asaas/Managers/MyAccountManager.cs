using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Common.Base;
using Codout.Apis.Asaas.Models.MyAccount;

namespace Codout.Apis.Asaas.Managers;

public class MyAccountManager(ApiSettings settings) : BaseManager(settings)
{
    private const string MyAccountRoute = "/myAccount";
    private const string CommercialInfoRoute = MyAccountRoute + "/commercialInfo";
    private const string PaymentCheckoutConfigRoute = MyAccountRoute + "/paymentCheckoutConfig";
    private const string FeesRoute = MyAccountRoute + "/fees";
    private const string AccountNumberRoute = MyAccountRoute + "/accountNumber";
    private const string StatusRoute = MyAccountRoute + "/status";
    private const string DocumentsRoute = MyAccountRoute + "/documents";

    public async Task<ResponseObject<MyAccount>> GetCommercialInfo()
    {
        return await GetAsync<MyAccount>(CommercialInfoRoute);
    }

    public async Task<ResponseObject<MyAccount>> UpdateCommercialInfo(UpdateCommercialInfoRequest requestObj)
    {
        return await PostAsync<MyAccount>(CommercialInfoRoute, requestObj);
    }

    public async Task<ResponseObject<AccountStatus>> GetStatus()
    {
        return await GetAsync<AccountStatus>(StatusRoute);
    }

    public async Task<ResponseObject<BaseDeleted>> DeleteWhiteLabelAccount()
    {
        return await DeleteAsync<BaseDeleted>(MyAccountRoute);
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

    public async Task<ResponseList<AccountDocumentSection>> ListPendingDocuments(int offset = 0, int limit = 100)
    {
        return await GetListAsync<AccountDocumentSection>(DocumentsRoute, offset, limit);
    }

    public async Task<ResponseObject<AccountDocumentSection>> SubmitDocument(string documentId, UploadAccountDocumentRequest requestObj)
    {
        var route = $"{DocumentsRoute}/{documentId}";
        return await PostMultipartFormDataContentAsync<AccountDocumentSection>(route, requestObj);
    }

    public async Task<ResponseObject<AccountDocumentFile>> ViewDocumentFile(string fileId)
    {
        var route = $"{DocumentsRoute}/files/{fileId}";
        return await GetAsync<AccountDocumentFile>(route);
    }

    public async Task<ResponseObject<AccountDocumentFile>> UpdateDocumentFile(string fileId, UploadAccountDocumentRequest requestObj)
    {
        var route = $"{DocumentsRoute}/files/{fileId}";
        return await PostMultipartFormDataContentAsync<AccountDocumentFile>(route, requestObj);
    }

    public async Task<ResponseObject<BaseDeleted>> DeleteDocumentFile(string fileId)
    {
        var route = $"{DocumentsRoute}/files/{fileId}";
        return await DeleteAsync<BaseDeleted>(route);
    }
}
