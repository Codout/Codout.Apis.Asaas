using System.Collections.Generic;

namespace Codout.Apis.Asaas.Models.MyAccount;

public class AccountDocumentResponse
{
    public string RejectReasons { get; set; }
    public List<AccountDocumentGroup> Data { get; set; } = [];
}
