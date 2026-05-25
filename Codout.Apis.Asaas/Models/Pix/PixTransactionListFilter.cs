using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Models.Pix.Enums;

namespace Codout.Apis.Asaas.Models.Pix;

public class PixTransactionListFilter : RequestParameters
{
    public PixTransactionStatus? Status
    {
        get => Get<PixTransactionStatus?>("status");
        set => Add("status", value);
    }

    public PixTransactionType? Type
    {
        get => Get<PixTransactionType?>("type");
        set => Add("type", value);
    }

    public string EndToEndIdentifier
    {
        get => this["endToEndIdentifier"];
        set => Add("endToEndIdentifier", value);
    }
}
