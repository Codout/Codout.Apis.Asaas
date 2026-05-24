using System;

namespace Codout.Apis.Asaas.Models.PixRecurring;

public class PixRecurringTransaction
{
    public string Id { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public decimal Value { get; set; }
    public string Periodicity { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? DateCreated { get; set; }
}

public class PixRecurringItem
{
    public string Id { get; set; }
    public string Status { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public decimal Value { get; set; }
    public string Description { get; set; }
}
