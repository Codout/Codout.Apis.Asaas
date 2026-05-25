namespace Codout.Apis.Asaas.Models.Payment;

public class SimulatedPaymentBankSlip
{
    public decimal NetValue { get; set; }
    public decimal FeeValue { get; set; }
    public SimulatedPaymentInstallment Installment { get; set; }
}
