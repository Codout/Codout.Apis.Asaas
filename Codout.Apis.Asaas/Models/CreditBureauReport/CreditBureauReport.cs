using System;

namespace Codout.Apis.Asaas.Models.CreditBureauReport
{
    public class CreditBureauReport
    {
        public string Id { get; set; }
        public string Customer { get; set; }
        public string CpfCnpj { get; set; }
        public string State { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
