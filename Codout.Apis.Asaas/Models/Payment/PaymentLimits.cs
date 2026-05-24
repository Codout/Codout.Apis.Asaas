namespace Codout.Apis.Asaas.Models.Payment;

public class PaymentLimits
{
    public PaymentLimitsItem CreditCard { get; set; }
    public PaymentLimitsItem Pix { get; set; }
    public PaymentLimitsItem BankSlip { get; set; }
}

public class PaymentLimitsItem
{
    public decimal? Daily { get; set; }
    public decimal? Monthly { get; set; }
    public decimal? AverageTicket { get; set; }
}
