using System;
using Codout.Apis.Asaas.Models.PixAutomatic.Enums;

namespace Codout.Apis.Asaas.Models.PixAutomatic;

public class PixAutomaticAuthorization
{
    public string Id { get; set; }
    public decimal? MinLimitValue { get; set; }
    public DateTime? CancellationDate { get; set; }
    public string CancellationReason { get; set; }
    public string ContractId { get; set; }
    public string CustomerId { get; set; }
    public string Description { get; set; }
    public DateTime? FinishDate { get; set; }
    public PixAutomaticRecurringFrequency? Frequency { get; set; }
    public string EndToEndIdentifier { get; set; }
    public DateTime? StartDate { get; set; }
    public PixAutomaticAuthorizationStatus Status { get; set; }
    public decimal? Value { get; set; }
    public string Payload { get; set; }
    public string EncodedImage { get; set; }
    public PixAutomaticImmediateQrCode ImmediateQrCode { get; set; }
    public PixAutomaticOriginType? OriginType { get; set; }
    public string SubscriptionId { get; set; }
}
