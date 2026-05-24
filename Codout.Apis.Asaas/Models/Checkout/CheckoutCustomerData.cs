namespace Codout.Apis.Asaas.Models.Checkout;

public class CheckoutCustomerData
{
    public string Name { get; set; }
    public string CpfCnpj { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public int? AddressNumber { get; set; }
    public string Complement { get; set; }
    public string Province { get; set; }
    public string PostalCode { get; set; }
    public int? City { get; set; }
}
