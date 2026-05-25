namespace Codout.Apis.Asaas.Models.Anticipation;

public class AnticipationLimits
{
    public AnticipationLimitsItem BankSlip { get; set; }
    public AnticipationLimitsItem CreditCard { get; set; }
}

public class AnticipationLimitsItem
{
    public decimal Total { get; set; }
    public decimal Available { get; set; }
    public decimal Used { get; set; }
}
