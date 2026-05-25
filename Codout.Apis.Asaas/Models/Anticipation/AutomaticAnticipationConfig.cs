namespace Codout.Apis.Asaas.Models.Anticipation;

public class AutomaticAnticipationConfig
{
    public bool BankSlipEnabled { get; set; }
    public bool CreditCardEnabled { get; set; }
}

public class UpdateAutomaticAnticipationConfigRequest
{
    public bool? BankSlipEnabled { get; set; }
    public bool? CreditCardEnabled { get; set; }
}
