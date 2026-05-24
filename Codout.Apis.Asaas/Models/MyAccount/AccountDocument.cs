using Codout.Apis.Asaas.Models.MyAccount.Enums;

namespace Codout.Apis.Asaas.Models.MyAccount;

public class AccountDocument
{
    public string Id { get; set; }
    public AccountDocumentStatus Status { get; set; }
}
