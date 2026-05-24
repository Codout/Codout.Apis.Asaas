using System;

namespace Codout.Apis.Asaas.Models.PixAutomatic;

public class PixAutomaticPaymentInstruction
{
    public string Id { get; set; }
    public string Authorization { get; set; }
    public string Status { get; set; }
    public decimal Value { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? DateCreated { get; set; }
    public string Description { get; set; }
}
