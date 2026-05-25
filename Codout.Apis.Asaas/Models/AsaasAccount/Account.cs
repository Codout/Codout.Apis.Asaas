using System;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.MyAccount;

namespace Codout.Apis.Asaas.Models.AsaasAccount;

public class Account
{
    public string Object { get; set; }

    public string Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string LoginEmail { get; set; }

    public string Phone { get; set; }

    public string MobilePhone { get; set; }

    public string Address { get; set; }

    public string AddressNumber { get; set; }

    public string Complement { get; set; }

    public string Province { get; set; }

    public string PostalCode { get; set; }

    public string CpfCnpj { get; set; }

    public DateTime? BirthDate { get; set; }

    public PersonType? PersonType { get; set; }

    public CompanyType? CompanyType { get; set; }

    /// <summary>
    /// Schema oficial: integer (city id). Antes era string.
    /// </summary>
    public long? City { get; set; }

    public string State { get; set; }

    public string Country { get; set; }

    public string TradingName { get; set; }

    public string Site { get; set; }

    public string WalletId { get; set; }

    public AccountNumber AccountNumber { get; set; }

    public CommercialInfoExpiration CommercialInfoExpiration { get; set; }

    /// <summary>
    /// Mantido por backwards-compat. NAO existe na response schema oficial
    /// (apenas retornado em raros endpoints legados). Pode vir null.
    /// </summary>
    [Obsolete("Nao existe no schema AccountGetResponseDTO.")]
    public string ApiKey { get; set; }
}
