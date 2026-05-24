using System;
using System.Text.Json.Serialization;
using Codout.Apis.Asaas.Models.Chargeback.Enums;

namespace Codout.Apis.Asaas.Models.Chargeback;

public class Chargeback
{
    public string Id { get; set; }

    [JsonPropertyName("payment")]
    public string PaymentId { get; set; }

    [JsonPropertyName("installment")]
    public string InstallmentId { get; set; }

    [JsonPropertyName("customerAccount")]
    public string CustomerAccountId { get; set; }

    public ChargebackStatus Status { get; set; }

    public ChargebackReason Reason { get; set; }

    public DateTime? DisputeStartDate { get; set; }

    public decimal Value { get; set; }

    public DateTime? PaymentDate { get; set; }

    public ChargebackDisputeStatus? DisputeStatus { get; set; }

    public DateTime? DeadlineToSendDisputeDocuments { get; set; }
}
