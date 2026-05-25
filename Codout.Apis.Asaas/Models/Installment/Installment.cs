using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Codout.Apis.Asaas.Models.Common;
using Codout.Apis.Asaas.Models.Common.Enums;

namespace Codout.Apis.Asaas.Models.Installment;

public class Installment
{
    [JsonPropertyName("object")]
    public string Object { get; set; }

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
    public int? ExpirationDay { get; set; }

    [JsonPropertyName("dateCreated")]
    public DateTime? DateCreated { get; set; }

    [JsonPropertyName("customer")]
    public string CustomerId { get; set; }

    [JsonPropertyName("deleted")]
    public bool? Deleted { get; set; }

    [JsonPropertyName("paymentLink")]
    public string PaymentLink { get; set; }

    [JsonPropertyName("checkoutSession")]
    public string CheckoutSession { get; set; }

    [JsonPropertyName("transactionReceiptUrl")]
    public string TransactionReceiptUrl { get; set; }

    [JsonPropertyName("creditCard")]
    public Common.CreditCard CreditCard { get; set; }

    /// <summary>
    /// Schema retorna array de refunds (mesma shape do PaymentRefund + paymentId).
    /// </summary>
    [JsonPropertyName("refunds")]
    public List<InstallmentRefund> Refunds { get; set; } = [];
}

public class InstallmentRefund
{
    public DateTime? DateCreated { get; set; }
    public string Status { get; set; }
    public decimal Value { get; set; }
    public string EndToEndIdentifier { get; set; }
    public string Description { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public string TransactionReceiptUrl { get; set; }
    public string PaymentId { get; set; }
}
