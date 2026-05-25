using System;

namespace Codout.Apis.Asaas.Models.Pix;

public class PixTransactionQrCode
{
    public PixTransactionQrCodePayer Payer { get; set; }
    public string ConciliationIdentifier { get; set; }
    public decimal? OriginalValue { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? Interest { get; set; }
    public decimal? Fine { get; set; }
    public decimal? Discount { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string Description { get; set; }
}

public class PixTransactionQrCodePayer
{
    public string Name { get; set; }
    public string CpfCnpj { get; set; }
}
