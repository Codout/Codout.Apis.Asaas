using System;
using Codout.Apis.Asaas.Models.PixRecurring.Enums;

namespace Codout.Apis.Asaas.Models.PixRecurring;

public class PixRecurringItem
{
    public string Id { get; set; }
    public PixRecurringItemStatus Status { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public bool? CanBeCancelled { get; set; }
    public int? RecurrenceNumber { get; set; }
    public int? Quantity { get; set; }
    public decimal Value { get; set; }
    public string RefusalReasonDescription { get; set; }
    public PixRecurringExternalAccount ExternalAccount { get; set; }
}
