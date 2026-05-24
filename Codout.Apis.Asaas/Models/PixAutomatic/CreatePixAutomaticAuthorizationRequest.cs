using System;
using Codout.Apis.Asaas.Models.PixAutomatic.Enums;

namespace Codout.Apis.Asaas.Models.PixAutomatic;

public class CreatePixAutomaticAuthorizationRequest
{
    public PixAutomaticRecurringFrequency Frequency { get; set; }
    public string ContractId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? FinishDate { get; set; }
    public decimal? Value { get; set; }
    public string Description { get; set; }
    public string CustomerId { get; set; }
    public CreatePixAutomaticImmediateQrCodeRequest ImmediateQrCode { get; set; }
    public decimal? MinLimitValue { get; set; }
    public PixAutomaticPaymentCreationMode? PaymentCreationMode { get; set; }
}
