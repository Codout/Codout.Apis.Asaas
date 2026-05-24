using System;
using System.Collections.Generic;
using Codout.Apis.Asaas.Models.Bill.Enums;

namespace Codout.Apis.Asaas.Models.Bill;

public class BillPayment
{
    public string Id { get; set; }

    public BillPaymentStatus Status { get; set; }

    public decimal Value { get; set; }

    public decimal Discount { get; set; }

    public decimal Interest { get; set; }

    public decimal Fine { get; set; }

    public string IdentificationField { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? ScheduleDate { get; set; }

    public DateTime? PaymentDate { get; set; }

    public decimal Fee { get; set; }

    public string Description { get; set; }

    public string CompanyName { get; set; }

    public string TransactionReceiptUrl { get; set; }

    public bool? CanBeCancelled { get; set; }

    public string ExternalReference { get; set; }

    /// <summary>
    /// Schema retorna array de strings com os motivos da falha.
    /// </summary>
    public List<string> FailReasons { get; set; } = [];
}
