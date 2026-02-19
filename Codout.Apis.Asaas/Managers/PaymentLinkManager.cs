using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Common.Base;
using Codout.Apis.Asaas.Models.PaymentLink;

namespace Codout.Apis.Asaas.Managers;

public class PaymentLinkManager(ApiSettings settings) : BaseManager(settings)
{
    private const string PaymentLinksRoute = "/paymentLinks";

    public async Task<ResponseObject<PaymentLink>> Create(CreatePaymentLinkRequest requestObj)
    {
        return await PostAsync<PaymentLink>(PaymentLinksRoute, requestObj);
    }

    public async Task<ResponseList<PaymentLink>> List(int offset, int limit)
    {
        return await GetListAsync<PaymentLink>(PaymentLinksRoute, offset, limit);
    }

    public async Task<ResponseObject<PaymentLink>> Find(string paymentLinkId)
    {
        var route = $"{PaymentLinksRoute}/{paymentLinkId}";
        return await GetAsync<PaymentLink>(route);
    }

    public async Task<ResponseObject<PaymentLink>> Update(string paymentLinkId, UpdatePaymentLinkRequest requestObj)
    {
        var route = $"{PaymentLinksRoute}/{paymentLinkId}";
        return await PutAsync<PaymentLink>(route, requestObj);
    }

    public async Task<ResponseObject<BaseDeleted>> Delete(string paymentLinkId)
    {
        var route = $"{PaymentLinksRoute}/{paymentLinkId}";
        return await DeleteAsync<BaseDeleted>(route);
    }

    public async Task<ResponseObject<PaymentLink>> Restore(string paymentLinkId)
    {
        var route = $"{PaymentLinksRoute}/{paymentLinkId}/restore";
        return await PostAsync<PaymentLink>(route, new RequestParameters());
    }

    public async Task<ResponseObject<PaymentLinkImage>> AddImage(string paymentLinkId, AddPaymentLinkImageRequest requestObj)
    {
        var route = $"{PaymentLinksRoute}/{paymentLinkId}/images";
        return await PostMultipartFormDataContentAsync<PaymentLinkImage>(route, requestObj);
    }

    public async Task<ResponseList<PaymentLinkImage>> ListImages(string paymentLinkId, int offset, int limit)
    {
        var route = $"{PaymentLinksRoute}/{paymentLinkId}/images";
        return await GetListAsync<PaymentLinkImage>(route, offset, limit);
    }

    public async Task<ResponseObject<BaseDeleted>> DeleteImage(string paymentLinkId, string imageId)
    {
        var route = $"{PaymentLinksRoute}/{paymentLinkId}/images/{imageId}";
        return await DeleteAsync<BaseDeleted>(route);
    }

    public async Task<ResponseObject<PaymentLinkImage>> FindImage(string paymentLinkId, string imageId)
    {
        var route = $"{PaymentLinksRoute}/{paymentLinkId}/images/{imageId}";
        return await GetAsync<PaymentLinkImage>(route);
    }

    public async Task<ResponseObject<PaymentLinkImage>> SetMainImage(string paymentLinkId, string imageId)
    {
        var route = $"{PaymentLinksRoute}/{paymentLinkId}/images/{imageId}/setAsMain";
        return await PostAsync<PaymentLinkImage>(route, new RequestParameters());
    }
}
