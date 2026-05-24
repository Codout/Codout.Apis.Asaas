namespace Codout.Apis.Asaas.Models.Payment;

public class PaymentBillingInfo
{
    public PaymentBillingInfoPix Pix { get; set; }
    public PaymentBillingInfoCreditCard CreditCard { get; set; }
    public PaymentBillingInfoBankSlip BankSlip { get; set; }
}

public class PaymentBillingInfoCreditCard
{
    public string CreditCardNumber { get; set; }
    public string CreditCardBrand { get; set; }
    public string CreditCardToken { get; set; }
}

public class PaymentBillingInfoPix
{
    public string EncodedImage { get; set; }
    public string Payload { get; set; }
    public System.DateTime? ExpirationDate { get; set; }
    public string Description { get; set; }
}

public class PaymentBillingInfoBankSlip
{
    public string IdentificationField { get; set; }
    public string NossoNumero { get; set; }
    public string BarCode { get; set; }
    public string BankSlipUrl { get; set; }
    public int? DaysAfterDueDateToRegistrationCancellation { get; set; }
}
