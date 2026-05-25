using System;
using System.Collections.Generic;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.MyAccount.Enums;

namespace Codout.Apis.Asaas.Models.MyAccount;

public class MyAccount
{
    public AccountInfoStatus? Status { get; set; }

    public PersonType? PersonType { get; set; }

    public string CpfCnpj { get; set; }

    public string Name { get; set; }

    public DateTime? BirthDate { get; set; }

    public string CompanyName { get; set; }

    public CompanyType? CompanyType { get; set; }

    public decimal? IncomeValue { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public string MobilePhone { get; set; }

    public string PostalCode { get; set; }

    public string Address { get; set; }

    public string AddressNumber { get; set; }

    public string Complement { get; set; }

    public string Province { get; set; }

    public City City { get; set; }

    public string DenialReason { get; set; }

    public string TradingName { get; set; }

    public string Site { get; set; }

    public List<string> AvailableCompanyNames { get; set; } = [];

    public CommercialInfoExpiration CommercialInfoExpiration { get; set; }

    /// <summary>
    /// Mantido por backwards-compat. Nao existe no schema atual.
    /// </summary>
    [Obsolete("Nao existe no schema oficial AccountInfoGetResponseDTO.")]
    public string InscricaoEstadual { get; set; }
}
