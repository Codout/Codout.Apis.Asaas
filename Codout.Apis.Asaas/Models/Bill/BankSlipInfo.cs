using System;

namespace Codout.Apis.Asaas.Models.Bill;

public class BankSlipInfo
{
    public string IdentificationField { get; set; }

    public decimal Value { get; set; }

    public DateTime? DueDate { get; set; }

    public string CompanyName { get; set; }

    /// <summary>
    /// Schema oficial: "bank" (codigo do banco). Antes era "bankCode" (inventado).
    /// </summary>
    public string Bank { get; set; }

    public string BeneficiaryCpfCnpj { get; set; }

    public string BeneficiaryName { get; set; }

    public bool AllowChangeValue { get; set; }

    public decimal MinValue { get; set; }

    public decimal MaxValue { get; set; }

    public decimal DiscountValue { get; set; }

    public decimal InterestValue { get; set; }

    public decimal FineValue { get; set; }

    public decimal OriginalValue { get; set; }

    public decimal TotalDiscountValue { get; set; }

    public decimal TotalAdditionalValue { get; set; }

    public bool IsOverdue { get; set; }
}
