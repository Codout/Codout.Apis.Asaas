using System;

namespace Codout.Apis.Asaas.Models.Payment;

public class PaymentSplitView
{
    public string Id { get; set; }
    public string Payment { get; set; }
    public string WalletId { get; set; }
    public decimal? FixedValue { get; set; }
    public decimal? PercentualValue { get; set; }
    public decimal? TotalValue { get; set; }
    public string Status { get; set; }
    public DateTime? CreditDate { get; set; }
    public string ExternalReference { get; set; }
    public string Description { get; set; }
}
