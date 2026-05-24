namespace Codout.Apis.Asaas.Models.Payment;

public class PaymentBillingInfo
{
    public PaymentBillingInfoCreditCard CreditCard { get; set; }
    public PaymentBillingInfoPix Pix { get; set; }
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
    public string ExpirationDate { get; set; }
}

public class PaymentBillingInfoBankSlip
{
    public string IdentificationField { get; set; }
    public string Nossonumero { get; set; }
    public string BarCode { get; set; }
}
