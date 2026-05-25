using System.Collections.Generic;
using Codout.Apis.Asaas.Models.MyAccount.Enums;

namespace Codout.Apis.Asaas.Models.MyAccount;

public class AccountDocumentResponsible
{
    public string Name { get; set; }
    public List<AccountDocumentResponsibleType> Type { get; set; } = [];
}
