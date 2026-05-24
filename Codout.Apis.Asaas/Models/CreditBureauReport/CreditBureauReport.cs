using System;

namespace Codout.Apis.Asaas.Models.CreditBureauReport;

public class CreditBureauReport
{
    public string Id { get; set; }

    public DateTime? DateCreated { get; set; }

    public string CpfCnpj { get; set; }

    public string Customer { get; set; }

    public string DownloadUrl { get; set; }

    /// <summary>
    /// PDF do relatorio em Base64. Retornado apenas quando o report e criado
    /// (POST). Em GET por id e nos itens do List, vem null.
    /// </summary>
    public string ReportFile { get; set; }
}
