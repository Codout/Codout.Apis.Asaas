using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Codout.Apis.Asaas.Models.Common;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Payment;
using Codout.Apis.Asaas.Models.Subscription.Enums;

namespace Codout.Apis.Asaas.Models.Subscription;

public class Subscription
{
    public string Object { get; set; }

    public string Id { get; set; }

    public DateTime? DateCreated { get; set; }

    [JsonPropertyName("customer")]
    public string CustomerId { get; set; }

    [JsonPropertyName("paymentLink")]
    public string PaymentLinkId { get; set; }

    public BillingType BillingType { get; set; }

    public decimal Value { get; set; }

    public DateTime? NextDueDate { get; set; }

    public Discount Discount { get; set; }

    public Interest Interest { get; set; }

    public Fine Fine { get; set; }

    public Cycle Cycle { get; set; }

    public string Description { get; set; }

    public DateTime? EndDate { get; set; }

    public int? MaxPayments { get; set; }

    public SubscriptionStatus Status { get; set; }

    public string ExternalReference { get; set; }

    public string CheckoutSession { get; set; }

    public Common.CreditCard CreditCard { get; set; }

    public bool? Deleted { get; set; }

    public List<Split> Split { get; set; } = [];
}
