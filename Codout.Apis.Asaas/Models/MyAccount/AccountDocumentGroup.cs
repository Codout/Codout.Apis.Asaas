using System;
using System.Collections.Generic;

namespace Codout.Apis.Asaas.Models.MyAccount;

public class AccountDocumentGroup
{
    public string Id { get; set; }
    public string Status { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public AccountDocumentResponsible Responsible { get; set; }
    public string OnboardingUrl { get; set; }
    public DateTime? OnboardingUrlExpirationDate { get; set; }
    public List<AccountDocument> Documents { get; set; } = [];
}
