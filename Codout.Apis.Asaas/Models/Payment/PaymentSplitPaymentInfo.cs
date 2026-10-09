using System;

namespace Codout.Apis.Asaas.Models.Payment;

/// <summary>Dados da cobrança associada a um split pago ou recebido.</summary>
public class PaymentSplitPaymentInfo
{
    public string Id { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public string InvoiceNumber { get; set; }
    public string ExternalReference { get; set; }
}
