using System;

namespace Codout.Apis.Asaas.Models.Payment;

public class PaymentRefund
{
    public DateTime? DateCreated { get; set; }
    public string Status { get; set; }
    public decimal Value { get; set; }
    public string EndToEndIdentifier { get; set; }
    public string Description { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public string TransactionReceiptUrl { get; set; }
}
