namespace Codout.Apis.Asaas.Models.Checkout;

public class CheckoutItem
{
    public string ExternalReference { get; set; }
    public string Description { get; set; }
    public string ImageBase64 { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal Value { get; set; }
}
