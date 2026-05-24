namespace Codout.Apis.Asaas.Models.Payment;

public class SimulatedPayment
{
    public decimal Value { get; set; }
    public SimulatedPaymentCreditCard CreditCard { get; set; }
    public SimulatedPaymentBankSlip BankSlip { get; set; }
    public SimulatedPaymentPix Pix { get; set; }
}
