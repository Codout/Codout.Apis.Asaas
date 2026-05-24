using Codout.Apis.Asaas.Core;

namespace Codout.Apis.Asaas.Models.PixAutomatic;

public class PixAutomaticPaymentInstructionListFilter : RequestParameters
{
    public string Authorization
    {
        get => this["authorization"];
        set => Add("authorization", value);
    }
    public string Status
    {
        get => this["status"];
        set => Add("status", value);
    }
}
