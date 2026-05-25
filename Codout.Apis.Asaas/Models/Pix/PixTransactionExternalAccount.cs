using Codout.Apis.Asaas.Models.Pix.Enums;

namespace Codout.Apis.Asaas.Models.Pix;

public class PixTransactionExternalAccount
{
    public string Ispb { get; set; }
    public string IspbName { get; set; }
    public string Name { get; set; }
    public string CpfCnpj { get; set; }
    public string AddressKey { get; set; }
    public PixAddressKeyType? AddressKeyType { get; set; }
}
