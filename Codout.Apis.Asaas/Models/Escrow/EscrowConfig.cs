namespace Codout.Apis.Asaas.Models.Escrow;

public class EscrowConfig
{
    public int DaysToExpire { get; set; }
    public bool? Enabled { get; set; }
    public bool? IsFeePayer { get; set; }
}
