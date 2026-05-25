using System;

namespace Codout.Apis.Asaas.Models.Payment;

public class PaymentViewingInfo
{
    public DateTime? BankSlipViewedDate { get; set; }
    public DateTime? PaymentCheckoutViewedDate { get; set; }
}
