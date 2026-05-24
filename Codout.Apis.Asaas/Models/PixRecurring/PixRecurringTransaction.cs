using System;
using Codout.Apis.Asaas.Models.PixRecurring.Enums;

namespace Codout.Apis.Asaas.Models.PixRecurring;

public class PixRecurringTransaction
{
    public string Id { get; set; }
    public PixRecurringStatus Status { get; set; }
    public PixRecurringOrigin? Origin { get; set; }
    public decimal Value { get; set; }
    public PixRecurringFrequency? Frequency { get; set; }
    public int Quantity { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? FinishDate { get; set; }
    public bool? CanBeCancelled { get; set; }
    public PixRecurringExternalAccount ExternalAccount { get; set; }
}
