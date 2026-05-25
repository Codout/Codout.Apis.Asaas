using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Payment.Enums;

namespace Codout.Apis.Asaas.Models.PaymentDunning;

public class PaymentDunningPaymentAvailable
{
    [JsonPropertyName("payment")]
    public string PaymentId { get; set; }

    [JsonPropertyName("customer")]
    public string CustomerId { get; set; }

    public decimal Value { get; set; }

    public PaymentStatus Status { get; set; }

    public BillingType BillingType { get; set; }

    public DateTime DueDate { get; set; }

    // Schema retorna ARRAY (simulacao por tipo). Antes do fix B-22m era
    // objeto unico, o que causava InvalidCastException na deserializacao.
    public List<PaymentDunningTypeSimulations> TypeSimulations { get; set; } = [];
}
