using System;
using System.Text.Json.Serialization;
using Codout.Apis.Asaas.Models.Common.Enums;

namespace Codout.Apis.Asaas.Models.Installment;

public class Installment
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    [JsonPropertyName("netValue")]
    public decimal NetValue { get; set; }

    [JsonPropertyName("paymentValue")]
    public decimal PaymentValue { get; set; }

    [JsonPropertyName("installmentCount")]
    public int InstallmentCount { get; set; }

    [JsonPropertyName("billingType")]
    public BillingType BillingType { get; set; }

    [JsonPropertyName("paymentDate")]
    public DateTime? PaymentDate { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("expirationDay")]
    public int ExpirationDay { get; set; }

    [JsonPropertyName("customer")]
    public string CustomerId { get; set; }

    [JsonPropertyName("deleted")]
    public bool Deleted { get; set; }

    [JsonPropertyName("paymentLink")]
    public string PaymentLink { get; set; }

    [JsonPropertyName("transactionReceiptUrl")]
    public string TransactionReceiptUrl { get; set; }
}
