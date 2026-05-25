using System;
using Codout.Apis.Asaas.Models.Transfer.Enums;

namespace Codout.Apis.Asaas.Models.Transfer.Base;

public class BaseTransfer
{
    public string Object { get; set; }

    public string Id { get; set; }

    public TransferType Type { get; set; }

    public TransferOperationType? OperationType { get; set; }

    public DateTime? DateCreated { get; set; }

    public decimal Value { get; set; }

    public decimal NetValue { get; set; }

    public decimal TransferFee { get; set; }

    public DateTime? EffectiveDate { get; set; }

    public DateTime? ScheduleDate { get; set; }

    public string EndToEndIdentifier { get; set; }

    public bool? Authorized { get; set; }

    public string FailReason { get; set; }

    public string ExternalReference { get; set; }

    public string TransactionReceiptUrl { get; set; }

    public string Description { get; set; }

    public string Recurring { get; set; }
}
