using System;
using Codout.Apis.Asaas.Models.Common;

namespace Codout.Apis.Asaas.Models.Invoice
{
    public class UpdateInvoiceRequest
    {
        public string ServiceDescription { get; set; }

        public string Observations { get; set; }

        public decimal? Value { get; set; }

        public decimal? Deductions { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public string ExternalReference { get; set; }

        public Taxes Taxes { get; set; }
    }
}
