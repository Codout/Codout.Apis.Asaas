using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Codout.Apis.Asaas.Models.Common;
using Codout.Apis.Asaas.Models.Common.Enums;

namespace Codout.Apis.Asaas.Models.Installment;

public class CreateInstallmentRequest
{
    public int InstallmentCount { get; set; }

    [JsonPropertyName("customer")]
    public string CustomerId { get; set; }

    public decimal Value { get; set; }

    public decimal? TotalValue { get; set; }

    public BillingType BillingType { get; set; }

    public DateTime DueDate { get; set; }

    public string Description { get; set; }

    public bool? PostalService { get; set; }

    public int? DaysAfterDueDateToRegistrationCancellation { get; set; }

    public string PaymentExternalReference { get; set; }

    public Discount Discount { get; set; }

    public Interest Interest { get; set; }

    public Fine Fine { get; set; }

    public List<InstallmentSplitRequest> Splits { get; set; } = [];
}
