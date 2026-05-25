namespace Codout.Apis.Asaas.Models.Installment;

public class InstallmentSplitRequest
{
    public string WalletId { get; set; }

    public decimal? FixedValue { get; set; }

    public decimal? PercentualValue { get; set; }

    public decimal? TotalFixedValue { get; set; }

    public string ExternalReference { get; set; }

    public string Description { get; set; }

    public int? InstallmentNumber { get; set; }
}
