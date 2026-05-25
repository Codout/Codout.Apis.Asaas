using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Models.PixAutomatic.Enums;

namespace Codout.Apis.Asaas.Models.PixAutomatic;

public class PixAutomaticAuthorizationListFilter : RequestParameters
{
    public PixAutomaticAuthorizationStatus? Status
    {
        get => Get<PixAutomaticAuthorizationStatus?>("status");
        set => Add("status", value);
    }
    public string CustomerId
    {
        get => this["customerId"];
        set => Add("customerId", value);
    }
}
