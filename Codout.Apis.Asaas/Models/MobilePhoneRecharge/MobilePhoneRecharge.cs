using System;
using System.Collections.Generic;

namespace Codout.Apis.Asaas.Models.MobilePhoneRecharge;

public class MobilePhoneRecharge
{
    public string Id { get; set; }
    public string PhoneNumber { get; set; }
    public string Provider { get; set; }
    public decimal Value { get; set; }
    public string Status { get; set; }
    public DateTime? DateCreated { get; set; }
    public DateTime? ConfirmedDate { get; set; }
}

public class CreateMobilePhoneRechargeRequest
{
    public string PhoneNumber { get; set; }
    public decimal Value { get; set; }
}

public class MobilePhoneProvider
{
    public string Name { get; set; }
    public List<decimal> AvailableValues { get; set; } = [];
}
