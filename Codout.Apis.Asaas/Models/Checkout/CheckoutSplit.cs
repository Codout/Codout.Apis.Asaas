namespace Codout.Apis.Asaas.Models.Checkout;

public class CheckoutSplit
{
    public string WalletId { get; set; }
    public decimal? FixedValue { get; set; }
    public decimal? PercentageValue { get; set; }
    public decimal? TotalFixedValue { get; set; }
}
