namespace Codout.Apis.Asaas.Models.Payment;

public class PaymentLimits
{
    public PaymentLimitsCreation Creation { get; set; }
}

public class PaymentLimitsCreation
{
    public PaymentLimitsDaily Daily { get; set; }
}

public class PaymentLimitsDaily
{
    public long Limit { get; set; }
    public long Used { get; set; }
    public bool? WasReached { get; set; }
}
