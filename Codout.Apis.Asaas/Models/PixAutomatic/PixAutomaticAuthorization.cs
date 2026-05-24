using System;
using System.Collections.Generic;

namespace Codout.Apis.Asaas.Models.PixAutomatic;

public class PixAutomaticAuthorization
{
    public string Id { get; set; }
    public string Status { get; set; }
    public string Customer { get; set; }
    public string ContractId { get; set; }
    public string Description { get; set; }
    public DateTime? DateCreated { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string PayerCpfCnpj { get; set; }
    public string PayerName { get; set; }
}

public class CreatePixAutomaticAuthorizationRequest
{
    public string Customer { get; set; }
    public string ContractId { get; set; }
    public string Description { get; set; }
    public decimal? FixedValue { get; set; }
    public decimal? MaximumValue { get; set; }
    public string Periodicity { get; set; }
    public DateTime? ExpirationDate { get; set; }
}

public class PixAutomaticAuthorizationListFilter : Core.RequestParameters
{
    public string Status
    {
        get => this["status"];
        set => Add("status", value);
    }
    public string Customer
    {
        get => this["customer"];
        set => Add("customer", value);
    }
}

public class PixAutomaticPaymentInstruction
{
    public string Id { get; set; }
    public string Authorization { get; set; }
    public string Status { get; set; }
    public decimal Value { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? DateCreated { get; set; }
    public string Description { get; set; }
}

public class PixAutomaticPaymentInstructionListFilter : Core.RequestParameters
{
    public string Authorization
    {
        get => this["authorization"];
        set => Add("authorization", value);
    }
    public string Status
    {
        get => this["status"];
        set => Add("status", value);
    }
}
