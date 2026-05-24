namespace Codout.Apis.Asaas.Models.CreditCard;

public class PreAuthorizationConfig
{
    public bool Enabled { get; set; }
    public int? AutomaticCaptureDelay { get; set; }
}

public class SavePreAuthorizationConfigRequest
{
    public bool Enabled { get; set; }
    public int? AutomaticCaptureDelay { get; set; }
}
