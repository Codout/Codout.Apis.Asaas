namespace Codout.Apis.Asaas.Models.Payment;

public class SimulatedPaymentCreditCard
{
    public decimal NetValue { get; set; }
    public decimal FeePercentage { get; set; }
    public decimal OperationFee { get; set; }
    public SimulatedPaymentInstallment Installment { get; set; }
}
