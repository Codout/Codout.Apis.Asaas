using Codout.Apis.Asaas.Models.Common.Enums;

namespace Codout.Apis.Asaas.Models.AsaasAccount;

public class CreateAccountRequest
{
    public string Name { get; set; }

    public string Email { get; set; }

    public string LoginEmail { get; set; }

    public string CpfCnpj { get; set; }

    public CompanyType? CompanyType { get; set; }

    public string Phone { get; set; }

    public string MobilePhone { get; set; }

    public string Address { get; set; }

    public string AddressNumber { get; set; }

    public string Complement { get; set; }

    public string Province { get; set; }

    public string PostalCode { get; set; }

    /// <summary>
    /// Data de nascimento do titular (PF), formato yyyy-MM-dd. Obrigatorio para
    /// pessoa fisica; omitir para pessoa juridica.
    /// </summary>
    public string BirthDate { get; set; }

    /// <summary>
    /// Faturamento/renda mensal em BRL. Obrigatorio no Asaas desde 2024.
    /// </summary>
    public decimal? IncomeValue { get; set; }
}
