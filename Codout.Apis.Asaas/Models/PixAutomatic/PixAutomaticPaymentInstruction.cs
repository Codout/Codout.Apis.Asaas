using System;
using Codout.Apis.Asaas.Models.PixAutomatic.Enums;

namespace Codout.Apis.Asaas.Models.PixAutomatic;

public class PixAutomaticPaymentInstruction
{
    public string Id { get; set; }
    public string EndToEndIdentifier { get; set; }
    public PixAutomaticPaymentInstructionAuthorization Authorization { get; set; }
    public DateTime? DueDate { get; set; }
    public PixAutomaticPaymentInstructionStatus Status { get; set; }
    public string PaymentId { get; set; }
    public string RefusalReason { get; set; }
}
