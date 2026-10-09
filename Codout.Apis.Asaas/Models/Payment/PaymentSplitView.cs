using System;
using System.Text.Json.Serialization;
using Codout.Apis.Asaas.Core;

namespace Codout.Apis.Asaas.Models.Payment;

public class PaymentSplitView
{
    public string Id { get; set; }
    /// <summary>
    /// Identificador da cobrança, mantido para compatibilidade. Atribuir um valor
    /// substitui PaymentInfo por uma referência sem metadados; null limpa a referência.
    /// </summary>
    [JsonIgnore]
    public string Payment
    {
        get => PaymentInfo?.Id;
        set => PaymentInfo = value is null ? null : new PaymentSplitPaymentInfo { Id = value };
    }

    /// <summary>Dados tipados da cobrança. No JSON corresponde ao campo payment.</summary>
    [JsonPropertyName("payment")]
    [JsonConverter(typeof(PaymentSplitPaymentInfoConverter))]
    public PaymentSplitPaymentInfo PaymentInfo { get; set; }
    public string WalletId { get; set; }
    public decimal? FixedValue { get; set; }
    public decimal? PercentualValue { get; set; }
    public decimal? TotalValue { get; set; }
    public string Status { get; set; }
    public string CancellationReason { get; set; }
    public string OriginAccountId { get; set; }
    public string DestinationAccountId { get; set; }
    public DateTime? CreditDate { get; set; }
    public string ExternalReference { get; set; }
    public string Description { get; set; }
}
