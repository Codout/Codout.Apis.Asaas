namespace Codout.Apis.Asaas.Models.Payment;

public class PaymentBillingInfo
{
    public PaymentBillingInfoPix Pix { get; set; }
    public PaymentBillingInfoCreditCard CreditCard { get; set; }
    public PaymentBillingInfoBankSlip BankSlip { get; set; }
}
