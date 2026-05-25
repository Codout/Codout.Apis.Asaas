using System;
using System.Text.Json.Serialization;
using Codout.Apis.Asaas.Models.PaymentDunning.Enums;

namespace Codout.Apis.Asaas.Models.PaymentDunning;

public class PaymentDunning
{
    public string Id { get; set; }

    public int? DunningNumber { get; set; }

    public PaymentDunningStatus Status { get; set; }

    public PaymentDunningType Type { get; set; }

    [JsonPropertyName("payment")]
    public string PaymentId { get; set; }

    public DateTime RequestDate { get; set; }

    public string Description { get; set; }

    public decimal Value { get; set; }

    public decimal FeeValue { get; set; }

    public decimal NetValue { get; set; }

    [Obsolete("Campo deprecated no schema oficial.")]
    public decimal ReceivedInCashFeeValue { get; set; }

    public string DenialReason { get; set; }

    [Obsolete("Campo deprecated no schema oficial.")]
    public decimal CancellationFeeValue { get; set; }

    public bool? IsNecessaryResendDocumentation { get; set; }

    public bool? CanBeCancelled { get; set; }

    public string CannotBeCancelledReason { get; set; }
}
