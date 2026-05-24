using System.Collections.Generic;
using Codout.Apis.Asaas.Models.Checkout.Enums;

namespace Codout.Apis.Asaas.Models.Checkout;

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
