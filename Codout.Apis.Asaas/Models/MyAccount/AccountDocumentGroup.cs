using System;
using System.Collections.Generic;
using Codout.Apis.Asaas.Models.MyAccount.Enums;

namespace Codout.Apis.Asaas.Models.MyAccount;

public class AccountDocumentGroup
{
    public string Id { get; set; }
    public AccountDocumentGroupStatus Status { get; set; }
    public AccountDocumentType Type { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public AccountDocumentResponsible Responsible { get; set; }
    public string OnboardingUrl { get; set; }
    public DateTime? OnboardingUrlExpirationDate { get; set; }
    public List<AccountDocument> Documents { get; set; } = [];
}
