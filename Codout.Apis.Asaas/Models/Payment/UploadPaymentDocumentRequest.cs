using Codout.Apis.Asaas.Core.Interfaces;

namespace Codout.Apis.Asaas.Models.Payment;

public class UploadPaymentDocumentRequest
{
    public string Type { get; set; }

    public bool? Available { get; set; }

    public IAsaasFile File { get; set; }
}
