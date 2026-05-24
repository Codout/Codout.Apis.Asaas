using System;
using Codout.Apis.Asaas.Models.Subscription.Enums;

namespace Codout.Apis.Asaas.Models.Checkout;

public class CheckoutSubscription
{
    public Cycle Cycle { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? NextDueDate { get; set; }
}
