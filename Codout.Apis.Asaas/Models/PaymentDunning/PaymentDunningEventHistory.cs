using System;
using Codout.Apis.Asaas.Models.PaymentDunning.Enums;

namespace Codout.Apis.Asaas.Models.PaymentDunning;

public class PaymentDunningEventHistory
{
    public PaymentDunningHistoryStatus Status { get; set; }

    public string Description { get; set; }

    public DateTime EventDate { get; set; }
}
