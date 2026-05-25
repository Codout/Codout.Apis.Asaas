using System;
using Codout.Apis.Asaas.Models.Pix.Enums;

namespace Codout.Apis.Asaas.Models.Pix;

public class PixTransaction
{
    public string Id { get; set; }
    public string EndToEndIdentifier { get; set; }
    public PixTransactionFinality? Finality { get; set; }
    public decimal Value { get; set; }
    public decimal? ChangeValue { get; set; }
    public decimal? RefundedValue { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public PixTransactionStatus Status { get; set; }
    public PixTransactionType? Type { get; set; }
    public PixTransactionOriginType? OriginType { get; set; }
    public string ConciliationIdentifier { get; set; }
    public string Description { get; set; }
    public string TransactionReceiptUrl { get; set; }
    public string RefusalReason { get; set; }
    public bool? CanBeCanceled { get; set; }
    public PixOriginalTransaction OriginalTransaction { get; set; }
    public PixTransactionExternalAccount ExternalAccount { get; set; }
    public PixTransactionQrCode QrCode { get; set; }
    public string Payment { get; set; }
    public bool? CanBeRefunded { get; set; }
    public string RefundDisabledReason { get; set; }
    public decimal? ChargedFeeValue { get; set; }
    public DateTime? DateCreated { get; set; }
    public string AddressKey { get; set; }
    public PixAddressKeyType? AddressKeyType { get; set; }
    public string TransferId { get; set; }
    public string ExternalReference { get; set; }
}
