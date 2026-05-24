using System;
using System.Collections.Generic;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Payment;

namespace Codout.Apis.Asaas.Models.Checkout;

public class Checkout
{
    public string Id { get; set; }
    public string Status { get; set; }
    public decimal Value { get; set; }
    public DateTime? DueDate { get; set; }
    public string CheckoutUrl { get; set; }
    public string Customer { get; set; }
    public string CustomerData { get; set; }
    public DateTime? DateCreated { get; set; }
}

public class CreateCheckoutRequest
{
    public List<BillingType> BillingTypes { get; set; } = [];
    public List<string> ChargeTypes { get; set; } = [];
    public decimal Value { get; set; }
    public int? MinutesToExpire { get; set; }
    public DateTime? DueDate { get; set; }
    public string Description { get; set; }
    public string ExternalReference { get; set; }
    public CheckoutCustomerData CustomerData { get; set; }
    public CheckoutCallback Callback { get; set; }
    public List<Split> Splits { get; set; } = [];
}

public class CheckoutCustomerData
{
    public string Name { get; set; }
    public string CpfCnpj { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string AddressNumber { get; set; }
    public string Complement { get; set; }
    public string Province { get; set; }
    public string PostalCode { get; set; }
}

public class CheckoutCallback
{
    public string SuccessUrl { get; set; }
    public string CancelUrl { get; set; }
    public string ExpiredUrl { get; set; }
}
