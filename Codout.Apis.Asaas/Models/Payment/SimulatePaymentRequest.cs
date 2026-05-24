using System.Collections.Generic;
using Codout.Apis.Asaas.Models.Common.Enums;

namespace Codout.Apis.Asaas.Models.Payment;

public class SimulatePaymentRequest
{
    public decimal Value { get; set; }

    public BillingType BillingType { get; set; }

    public int? InstallmentCount { get; set; }

    public string DiscountValue { get; set; }

    public List<Split> Splits { get; set; } = [];
}
