using System;
using System.Collections.Generic;
using Codout.Apis.Asaas.Models.Checkout.Enums;
using Codout.Apis.Asaas.Models.Subscription.Enums;

namespace Codout.Apis.Asaas.Models.Checkout;

public class Checkout
{
    public string Id { get; set; }
    public string Link { get; set; }
    public CheckoutStatus Status { get; set; }
    public List<CheckoutBillingType> BillingTypes { get; set; } = [];
    public List<CheckoutChargeType> ChargeTypes { get; set; } = [];
    public int? MinutesToExpire { get; set; }
    public string ExternalReference { get; set; }
    public CheckoutCallback Callback { get; set; }
    public List<CheckoutItem> Items { get; set; } = [];
    public CheckoutCustomerData CustomerData { get; set; }
    public CheckoutSubscription Subscription { get; set; }
    public CheckoutInstallment Installment { get; set; }
    public List<CheckoutSplit> Split { get; set; } = [];
}

public class CheckoutCallback
{
    public string SuccessUrl { get; set; }
    public string CancelUrl { get; set; }
    public string ExpiredUrl { get; set; }
}

public class CheckoutItem
{
    public string ExternalReference { get; set; }
    public string Description { get; set; }
    public string ImageBase64 { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal Value { get; set; }
}

public class CheckoutCustomerData
{
    public string Name { get; set; }
    public string CpfCnpj { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public int? AddressNumber { get; set; }
    public string Complement { get; set; }
    public string Province { get; set; }
    public string PostalCode { get; set; }
    public int? City { get; set; }
}

public class CheckoutSubscription
{
    public Cycle Cycle { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? NextDueDate { get; set; }
}

public class CheckoutInstallment
{
    public int MaxInstallmentCount { get; set; }
}

public class CheckoutSplit
{
    public string WalletId { get; set; }
    public decimal? FixedValue { get; set; }
    public decimal? PercentageValue { get; set; }
    public decimal? TotalFixedValue { get; set; }
}

public class CreateCheckoutRequest
{
    public List<CheckoutBillingType> BillingTypes { get; set; } = [];
    public List<CheckoutChargeType> ChargeTypes { get; set; } = [];
    public int? MinutesToExpire { get; set; }
    public string ExternalReference { get; set; }
    public CheckoutCallback Callback { get; set; }
    public List<CheckoutItem> Items { get; set; } = [];
    public CheckoutCustomerData CustomerData { get; set; }
    public CheckoutSubscription Subscription { get; set; }
    public CheckoutInstallment Installment { get; set; }
    public List<CheckoutSplit> Splits { get; set; } = [];
}
