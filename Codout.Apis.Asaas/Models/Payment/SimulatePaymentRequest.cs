using System.Collections.Generic;
using Codout.Apis.Asaas.Models.Common.Enums;

namespace Codout.Apis.Asaas.Models.Payment;

public class SimulatePaymentRequest
{
    public decimal Value { get; set; }

    public List<BillingType> BillingTypes { get; set; } = [];

    public int? InstallmentCount { get; set; }
}
