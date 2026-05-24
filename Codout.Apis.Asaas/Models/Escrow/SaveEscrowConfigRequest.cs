namespace Codout.Apis.Asaas.Models.Escrow;

public class SaveEscrowConfigRequest
{
    public bool Enabled { get; set; }
    public int? DaysUntilExpire { get; set; }
}
