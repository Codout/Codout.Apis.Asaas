namespace Codout.Apis.Asaas.Models.PixAutomatic;

public class CreatePixAutomaticImmediateQrCodeRequest
{
    public string PixKey { get; set; }
    public int ExpirationSeconds { get; set; }
    public decimal OriginalValue { get; set; }
    public string Description { get; set; }
}
