namespace Codout.Apis.Asaas.Models.CreditCard;

/// <summary>
/// Schema oficial: apenas {daysToExpire: int required}. Antes tinha
/// campos inventados (Enabled, AutomaticCaptureDelay) que NAO existem
/// no schema CreditCardPreAuthorizationConfig.
/// </summary>
public class PreAuthorizationConfig
{
    public int DaysToExpire { get; set; }
}

public class SavePreAuthorizationConfigRequest
{
    public int DaysToExpire { get; set; }
}
