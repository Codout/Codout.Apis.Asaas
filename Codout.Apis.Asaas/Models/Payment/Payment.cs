using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Codout.Apis.Asaas.Models.Common;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Payment.Enums;

namespace Codout.Apis.Asaas.Models.Payment;

public class Payment
{
    public string Id { get; set; }

    public DateTime DateCreated { get; set; }

    [JsonPropertyName("customer")]
    public string CustomerId { get; set; }

    [JsonPropertyName("subscription")]
    public string SubscriptionId { get; set; }

    [JsonPropertyName("installment")]
    public string InstallmentId { get; set; }

    public DateTime DueDate { get; set; }

    public decimal Value { get; set; }

    public decimal NetValue { get; set; }

    public Discount Discount { get; set; }

    public Interest Interest { get; set; }

    public Fine Fine { get; set; }

    public BillingType BillingType { get; set; }

    public PaymentStatus Status { get; set; }

    public string Description { get; set; }

    public string ExternalReference { get; set; }

    public DateTime OriginalDueDate { get; set; }

    public decimal? OriginalValue { get; set; }

    public decimal? InterestValue { get; set; }

    public DateTime? ConfirmedDate { get; set; }

    public DateTime? PaymentDate { get; set; }

    public DateTime? ClientPaymentDate { get; set; }

    public string InvoiceUrl { get; set; }

    public string BankSlipUrl { get; set; }

    public string InvoiceNumber { get; set; }

    public bool? Deleted { get; set; }

    public bool? PostalService { get; set; }

    public bool? Anticipated { get; set; }

    public bool? Anticipable { get; set; }

    public bool? CanBePaidAfterDueDate { get; set; }

    public string Object { get; set; }

    public string PixTransaction { get; set; }

    public string PixQrCodeId { get; set; }

    public string CheckoutSession { get; set; }

    [JsonPropertyName("paymentLink")]
    public string PaymentLinkId { get; set; }

    public int? InstallmentNumber { get; set; }

    public DateTime? CreditDate { get; set; }

    public DateTime? EstimatedCreditDate { get; set; }

    public string TransactionReceiptUrl { get; set; }

    public string NossoNumero { get; set; }

    public int? DaysAfterDueDateToRegistrationCancellation { get; set; }

    public Common.CreditCard CreditCard { get; set; }

    public List<Split> Split { get; set; } = [];
}
