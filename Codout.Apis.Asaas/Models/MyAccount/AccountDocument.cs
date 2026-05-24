using System;
using System.Collections.Generic;
using Codout.Apis.Asaas.Core.Interfaces;

namespace Codout.Apis.Asaas.Models.MyAccount;

public class AccountDocumentSection
{
    public string Object { get; set; }
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public List<AccountDocument> Documents { get; set; } = [];
}

public class AccountDocument
{
    public string Object { get; set; }
    public string Id { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public DateTime? DateCreated { get; set; }
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
