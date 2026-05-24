using System;

namespace Codout.Apis.Asaas.Models.Escrow;

public class Escrow
{
    public string Id { get; set; }
    public string Status { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? FinishDate { get; set; }
    public string FinishReason { get; set; }
}

public class EscrowConfig
{
    public bool Enabled { get; set; }
    public int? DaysUntilExpire { get; set; }
}

public class SaveEscrowConfigRequest
{
    public bool Enabled { get; set; }
    public int? DaysUntilExpire { get; set; }
}

public class FinishEscrowRequest
{
    public string Reason { get; set; }
}
