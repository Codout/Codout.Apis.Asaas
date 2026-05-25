using System;

namespace Codout.Apis.Asaas.Models.Payment;

public class ReceiveInCashRequest
{
    public DateTime PaymentDate { get; set; }
    public decimal Value { get; set; }
    public bool NotifyCustomer { get; set; }
}
