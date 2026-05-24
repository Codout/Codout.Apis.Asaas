using System;

namespace Codout.Apis.Asaas.Models.Payment;

public class PaymentBillingInfoPix
{
    public string EncodedImage { get; set; }
    public string Payload { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string Description { get; set; }
}
