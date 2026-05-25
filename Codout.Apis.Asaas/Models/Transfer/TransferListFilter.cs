using System;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Models.Transfer.Enums;

namespace Codout.Apis.Asaas.Models.Transfer;

public class TransferListFilter : RequestParameters
{
    public DateTime? DateCreated
    {
        get => Get<DateTime?>("dateCreated");
        set => Add("dateCreated", value);
    }

    public DateTime? DateCreatedGE
    {
        get => Get<DateTime?>("dateCreated[ge]");
        set => Add("dateCreated[ge]", value);
    }

    public DateTime? DateCreatedLE
    {
        get => Get<DateTime?>("dateCreated[le]");
        set => Add("dateCreated[le]", value);
    }

    public DateTime? TransferDateGE
    {
        get => Get<DateTime?>("transferDate[ge]");
        set => Add("transferDate[ge]", value);
    }

    public DateTime? TransferDateLE
    {
        get => Get<DateTime?>("transferDate[le]");
        set => Add("transferDate[le]", value);
    }

    public TransferType? TransferType
    {
        get => Get<TransferType?>("type");
        set => Add("type", value);
    }
}
