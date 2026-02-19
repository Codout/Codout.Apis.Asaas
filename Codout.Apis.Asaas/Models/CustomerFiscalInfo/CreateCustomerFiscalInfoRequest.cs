using Codout.Apis.Asaas.Core.Interfaces;

namespace Codout.Apis.Asaas.Models.CustomerFiscalInfo
{
    public class CreateCustomerFiscalInfoRequest
    {
        public string Email { get; set; }
        public string MunicipalInscription { get; set; }
        public string StateInscription { get; set; }
        public bool SimplesNacional { get; set; }
        public bool CulturalProjectsPromoter { get; set; }
        public string Cnae { get; set; }
        public string SpecialTaxRegime { get; set; }
        public string ServiceListItem { get; set; }
        public string RpsSerie { get; set; }
        public string RpsNumber { get; set; }
        public string LoteNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AccessToken { get; set; }
        public IAsaasFile CertificateFile { get; set; }
        public string CertificatePassword { get; set; }
    }
}
