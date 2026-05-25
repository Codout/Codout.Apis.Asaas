namespace Codout.Apis.Asaas.Models.Payment;

public class PaymentLimitsDaily
{
    public long Limit { get; set; }
    public long Used { get; set; }
    public bool? WasReached { get; set; }
}
