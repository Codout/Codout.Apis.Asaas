namespace Codout.Apis.Asaas.Models.Payment;

public class PaymentBillingInfoBankSlip
{
    public string IdentificationField { get; set; }
    public string NossoNumero { get; set; }
    public string BarCode { get; set; }
    public string BankSlipUrl { get; set; }
    public int? DaysAfterDueDateToRegistrationCancellation { get; set; }
}
