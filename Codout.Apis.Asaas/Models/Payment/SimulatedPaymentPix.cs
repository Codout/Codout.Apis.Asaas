namespace Codout.Apis.Asaas.Models.Payment;

public class SimulatedPaymentPix
{
    public decimal NetValue { get; set; }
    public decimal? FeePercentage { get; set; }
    public decimal FeeValue { get; set; }
    public SimulatedPaymentInstallment Installment { get; set; }
}
