using System;

namespace Codout.Apis.Asaas.Models.Bill;

public class CreateBillPaymentRequest
{
    public string IdentificationField { get; set; }

    public DateTime? ScheduleDate { get; set; }

    public string Description { get; set; }

    public decimal? Discount { get; set; }

    public decimal? Interest { get; set; }

    public decimal? Fine { get; set; }

    public DateTime? DueDate { get; set; }

    public decimal? Value { get; set; }

    public string ExternalReference { get; set; }
}
