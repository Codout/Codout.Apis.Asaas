using System.Collections.Generic;
using Codout.Apis.Asaas.Models.Checkout.Enums;

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
