using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Common.Base;
using Codout.Apis.Asaas.Models.Pix;

namespace Codout.Apis.Asaas.Managers;

public class PixManager(ApiSettings settings) : BaseManager(settings)
{
    private const string PixRoute = "/pix";

    public async Task<ResponseList<PixTransaction>> ListTransactions(int offset, int limit)
    {
        var route = $"{PixRoute}/transactions";
        return await GetListAsync<PixTransaction>(route, offset, limit);
    }

    public async Task<ResponseObject<PixTransaction>> CancelTransaction(string transactionId)
    {
        var route = $"{PixRoute}/transactions/{transactionId}/cancel";
        return await PostAsync<PixTransaction>(route, new RequestParameters());
    }

    public async Task<ResponseObject<PixStaticQrCode>> CreateStaticQrCode(CreatePixStaticQrCodeRequest requestObj)
    {
        var route = $"{PixRoute}/qrCodes/static";
        return await PostAsync<PixStaticQrCode>(route, requestObj);
    }

    public async Task<ResponseObject<DecodedPixQrCode>> DecodeQrCode(DecodePixQrCodeRequest requestObj)
    {
        var route = $"{PixRoute}/qrCodes/decode";
        return await PostAsync<DecodedPixQrCode>(route, requestObj);
    }

    public async Task<ResponseObject<PixPayment>> PayQrCode(PayPixQrCodeRequest requestObj)
    {
        var route = $"{PixRoute}/qrCodes/pay";
        return await PostAsync<PixPayment>(route, requestObj);
    }

    public async Task<ResponseObject<PixAddressKey>> CreateAddressKey(CreatePixAddressKeyRequest requestObj)
    {
        var route = $"{PixRoute}/addressKeys";
        return await PostAsync<PixAddressKey>(route, requestObj);
    }

    public async Task<ResponseList<PixAddressKey>> ListAddressKeys(int offset, int limit)
    {
        var route = $"{PixRoute}/addressKeys";
        return await GetListAsync<PixAddressKey>(route, offset, limit);
    }

    public async Task<ResponseObject<PixAddressKey>> FindAddressKey(string addressKeyId)
    {
        var route = $"{PixRoute}/addressKeys/{addressKeyId}";
        return await GetAsync<PixAddressKey>(route);
    }

    public async Task<ResponseObject<BaseDeleted>> DeleteAddressKey(string addressKeyId)
    {
        var route = $"{PixRoute}/addressKeys/{addressKeyId}";
        return await DeleteAsync<BaseDeleted>(route);
    }
}
