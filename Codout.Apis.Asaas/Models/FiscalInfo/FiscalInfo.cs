using System;

namespace Codout.Apis.Asaas.Models.FiscalInfo;

public class FiscalInfo
{
    public string Object { get; set; }
    public string Email { get; set; }
    public string MunicipalInscription { get; set; }
    public bool? SimplesNacional { get; set; }
    public bool? CulturalProjectsPromoter { get; set; }
    public string Cnae { get; set; }
    public string SpecialTaxRegime { get; set; }
    public string ServiceListItem { get; set; }
    public string NbsCode { get; set; }
    public string RpsSerie { get; set; }

    /// <summary>Schema oficial: integer (era string).</summary>
    public int? RpsNumber { get; set; }

    /// <summary>Schema oficial: integer (era string).</summary>
    public int? LoteNumber { get; set; }

    public string Username { get; set; }

    public bool? PasswordSent { get; set; }

    public bool? AccessTokenSent { get; set; }

    public bool? CertificateSent { get; set; }

    public string NationalPortalTaxCalculationRegime { get; set; }

    /// <summary>NAO existe no schema. Removido/obsoleto.</summary>
    [Obsolete("Nao existe no schema FiscalInfoGetResponseDTO.")]
    public string StateInscription { get; set; }

    /// <summary>Use AccessTokenSent (bool). Campo legado.</summary>
    [Obsolete("Schema oficial expoe accessTokenSent (bool), nao accessToken (string).")]
    public string AccessToken { get; set; }
}
