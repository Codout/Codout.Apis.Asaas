using System;
using Codout.Apis.Asaas.Models.Escrow.Enums;

namespace Codout.Apis.Asaas.Models.Escrow;

public class Escrow
{
    public string Id { get; set; }
    public EscrowStatus Status { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? FinishDate { get; set; }
    public EscrowFinishReason? FinishReason { get; set; }
}
