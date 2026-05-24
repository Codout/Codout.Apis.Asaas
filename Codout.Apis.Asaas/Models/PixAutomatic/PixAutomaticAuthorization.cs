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

public class PixAutomaticImmediateQrCode
{
    public string ConciliationIdentifier { get; set; }
    public DateTime? ExpirationDate { get; set; }
}

public class CreatePixAutomaticAuthorizationRequest
{
    public PixAutomaticRecurringFrequency Frequency { get; set; }
    public string ContractId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? FinishDate { get; set; }
    public decimal? Value { get; set; }
    public string Description { get; set; }
    public string CustomerId { get; set; }
    public CreatePixAutomaticImmediateQrCodeRequest ImmediateQrCode { get; set; }
    public decimal? MinLimitValue { get; set; }
    public PixAutomaticPaymentCreationMode? PaymentCreationMode { get; set; }
}

public class CreatePixAutomaticImmediateQrCodeRequest
{
    public string PixKey { get; set; }
    public int ExpirationSeconds { get; set; }
    public decimal OriginalValue { get; set; }
    public string Description { get; set; }
}

public class PixAutomaticAuthorizationListFilter : Core.RequestParameters
{
    public PixAutomaticAuthorizationStatus? Status
    {
        get => Get<PixAutomaticAuthorizationStatus?>("status");
        set => Add("status", value);
    }
    public string CustomerId
    {
        get => this["customerId"];
        set => Add("customerId", value);
    }
}

public class PixAutomaticPaymentInstruction
{
    public string Id { get; set; }
    public string Authorization { get; set; }
    public string Status { get; set; }
    public decimal Value { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? DateCreated { get; set; }
    public string Description { get; set; }
}

public class PixAutomaticPaymentInstructionListFilter : Core.RequestParameters
{
    public string Authorization
    {
        get => this["authorization"];
        set => Add("authorization", value);
    }
    public string Status
    {
        get => this["status"];
        set => Add("status", value);
    }
}
