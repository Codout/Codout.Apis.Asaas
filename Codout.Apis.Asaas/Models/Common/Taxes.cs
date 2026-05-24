namespace Codout.Apis.Asaas.Models.Common;

/// <summary>
/// Modelo unificado para InvoiceTaxesRequestDTO e InvoiceTaxesResponseDTO.
/// Campos required do schema (retainIss, iss, pis, cofins, csll, inss, ir)
/// sao non-nullable. Demais campos sao opcionais (nullable). Os campos da
/// Reforma Tributaria (stateIbs/municipalIbs/cbs) aparecem apenas no response.
/// </summary>
public class Taxes
{
    public string NbsCode { get; set; }
    public string TaxSituationCode { get; set; }
    public string TaxClassificationCode { get; set; }
    public string OperationIndicatorCode { get; set; }

    public bool RetainIss { get; set; }
    public decimal Iss { get; set; }

    public string PisCofinsRetentionType { get; set; }
    public string PisCofinsTaxStatus { get; set; }

    public decimal Pis { get; set; }
    public decimal Cofins { get; set; }
    public decimal Csll { get; set; }
    public decimal Inss { get; set; }
    public decimal Ir { get; set; }

    public decimal? StateIbs { get; set; }
    public decimal? StateIbsValue { get; set; }
    public decimal? MunicipalIbs { get; set; }
    public decimal? MunicipalIbsValue { get; set; }
    public decimal? Cbs { get; set; }
    public decimal? CbsValue { get; set; }
}
