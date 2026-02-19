using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Common;
using Codout.Apis.Asaas.Models.CreditCard;

namespace Codout.Apis.Asaas.Managers;

public class CreditCardManager(ApiSettings settings) : BaseManager(settings)
{
    private const string PaymentsRoute = "/creditCard";

    public async Task<ResponseObject<CreditCard>> TokenizeCreditCard(TokenizeCreditCardRequest requestObj)
    {
        return await PostAsync<CreditCard>($"{PaymentsRoute}/tokenizeCreditCard", requestObj);
    }
}