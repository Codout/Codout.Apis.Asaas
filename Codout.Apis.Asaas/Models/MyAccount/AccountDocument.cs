using System;
using System.Collections.Generic;
using Codout.Apis.Asaas.Core.Interfaces;

namespace Codout.Apis.Asaas.Models.MyAccount;

public class AccountDocumentResponse
{
    public string RejectReasons { get; set; }
    public List<AccountDocumentGroup> Data { get; set; } = [];
}

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

public class AccountDocument
{
    public string Id { get; set; }
    public string Status { get; set; }
}

public class AccountDocumentResponsible
{
    public string Name { get; set; }
    public List<string> Type { get; set; } = [];
}

public class AccountDocumentFile
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
}

public class UploadAccountDocumentRequest
{
    public string DocumentType { get; set; }
    public IAsaasFile File { get; set; }
}
