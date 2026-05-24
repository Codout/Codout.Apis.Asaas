namespace Codout.Apis.Asaas.Models.Payment;

public class SimulatedPayment
{
    public decimal Value { get; set; }
    public SimulatedPaymentCreditCard CreditCard { get; set; }
    public SimulatedPaymentBankSlip BankSlip { get; set; }
    public SimulatedPaymentPix Pix { get; set; }
}

public class SimulatedPaymentInstallment
{
    public decimal PaymentNetValue { get; set; }
    public decimal PaymentValue { get; set; }
}

public class SimulatedPaymentCreditCard
{
    public decimal NetValue { get; set; }
    public decimal FeePercentage { get; set; }
    public decimal OperationFee { get; set; }
    public SimulatedPaymentInstallment Installment { get; set; }
}

public class SimulatedPaymentBankSlip
{
    public decimal NetValue { get; set; }
    public decimal FeeValue { get; set; }
    public SimulatedPaymentInstallment Installment { get; set; }
}

public class SimulatedPaymentPix
{
    public decimal NetValue { get; set; }
    public decimal? FeePercentage { get; set; }
    public decimal FeeValue { get; set; }
    public SimulatedPaymentInstallment Installment { get; set; }
}
