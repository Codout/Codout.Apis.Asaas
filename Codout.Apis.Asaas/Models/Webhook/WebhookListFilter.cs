using Codout.Apis.Asaas.Core;

namespace Codout.Apis.Asaas.Models.Webhook;

public class WebhookListFilter : RequestParameters
{
    public string Name
    {
        get => this["name"];
        set => Add("name", value);
    }

    public bool? Enabled
    {
        get => Get<bool?>("enabled");
        set => Add("enabled", value);
    }

    public bool? Interrupted
    {
        get => Get<bool?>("interrupted");
        set => Add("interrupted", value);
    }
}
