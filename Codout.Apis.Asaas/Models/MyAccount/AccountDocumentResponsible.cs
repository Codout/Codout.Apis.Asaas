using System.Collections.Generic;

namespace Codout.Apis.Asaas.Models.MyAccount;

public class AccountDocumentResponsible
{
    public string Name { get; set; }
    public List<string> Type { get; set; } = [];
}
