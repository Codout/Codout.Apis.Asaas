using Codout.Apis.Asaas.Models.Common.Enums;

namespace Codout.Apis.Asaas.Models.Chargeback;

public class ChargebackCreditCard
{
    public string Number { get; set; }
    public CreditCardBrand? Brand { get; set; }
}
