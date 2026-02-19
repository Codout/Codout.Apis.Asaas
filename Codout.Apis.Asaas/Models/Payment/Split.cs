namespace Codout.Apis.Asaas.Models.Payment;

public class Split
{
    public string WalletId { get; set; }

    public decimal? FixedValue { get; set; }
    
    public decimal? TotalFixedValue { get; set; }

    public decimal? PercentualValue { get; set; }
}
