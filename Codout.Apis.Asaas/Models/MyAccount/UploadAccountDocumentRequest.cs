using Codout.Apis.Asaas.Core.Interfaces;

namespace Codout.Apis.Asaas.Models.MyAccount;

public class UploadAccountDocumentRequest
{
    public string DocumentType { get; set; }
    public IAsaasFile File { get; set; }
}
