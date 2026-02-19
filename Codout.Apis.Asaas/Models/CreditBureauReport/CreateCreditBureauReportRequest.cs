namespace Codout.Apis.Asaas.Models.CreditBureauReport
{
    public class CreateCreditBureauReportRequest
    {
        public string Customer { get; set; }
        public string CpfCnpj { get; set; }
        public string State { get; set; }
    }
}
