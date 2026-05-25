using System;

namespace Codout.Apis.Asaas.Models.Payment;

public class PaymentDocument
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool Available { get; set; }
    public string Type { get; set; }
    public DateTime? DateCreated { get; set; }
}
