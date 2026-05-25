using Codout.Apis.Asaas.Models.Common;

namespace Codout.Apis.Asaas.Models.Payment;

public class PayWithCreditCardRequest
{
    public CreditCardRequest CreditCard { get; set; }
    public CreditCardHolderInfoRequest CreditCardHolderInfo { get; set; }
    public string CreditCardToken { get; set; }
    public string RemoteIp { get; set; }
}
