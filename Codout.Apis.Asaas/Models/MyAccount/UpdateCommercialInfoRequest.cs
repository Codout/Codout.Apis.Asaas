using System;
using Codout.Apis.Asaas.Models.Common.Enums;

namespace Codout.Apis.Asaas.Models.MyAccount;

public class UpdateCommercialInfoRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string CpfCnpj { get; set; }
    public DateTime? BirthDate { get; set; }
    public string CompanyName { get; set; }
    public CompanyType? CompanyType { get; set; }
    public decimal? IncomeValue { get; set; }
    public string Phone { get; set; }
    public string MobilePhone { get; set; }
    public string PostalCode { get; set; }
    public string Address { get; set; }
    public string AddressNumber { get; set; }
    public string Complement { get; set; }
    public string Province { get; set; }
    public string Site { get; set; }
}
