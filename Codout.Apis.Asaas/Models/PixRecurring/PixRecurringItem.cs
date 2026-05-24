using System;

namespace Codout.Apis.Asaas.Models.PixRecurring;

public class PixRecurringItem
{
    public string Id { get; set; }
    public string Status { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public decimal Value { get; set; }
    public string Description { get; set; }
}
