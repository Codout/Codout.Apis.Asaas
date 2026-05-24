using System.Collections.Generic;
using Codout.Apis.Asaas.Models.MobilePhoneRecharge.Enums;

namespace Codout.Apis.Asaas.Models.MobilePhoneRecharge;

public class MobilePhoneRecharge
{
    public string Id { get; set; }
    public decimal Value { get; set; }
    public string PhoneNumber { get; set; }
    public MobilePhoneRechargeStatus Status { get; set; }
    public bool? CanBeCancelled { get; set; }
    public string OperatorName { get; set; }
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
