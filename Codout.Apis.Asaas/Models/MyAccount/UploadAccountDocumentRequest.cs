using Codout.Apis.Asaas.Core.Interfaces;
using Codout.Apis.Asaas.Models.MyAccount.Enums;

namespace Codout.Apis.Asaas.Models.MyAccount;

/// <summary>
/// Request multipart/form-data para upload de documentos da conta.
/// Schema oficial: campos sao "documentFile" (binary) e "type" (enum).
/// Os nomes das propriedades do C# casam exatamente com o nome do form-field
/// apos a conversao firstCharToLower feita no PostMultipartFormDataContentAsync.
/// </summary>
public class UploadAccountDocumentRequest
{
    public AccountDocumentType? Type { get; set; }
    public IAsaasFile DocumentFile { get; set; }
}
