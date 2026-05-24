using System.Collections.Generic;
using Codout.Apis.Asaas.Models.Webhook.Enums;

namespace Codout.Apis.Asaas.Models.Webhook;

public class UpdateWebhookRequest
{
    public string Name { get; set; }

    public string Url { get; set; }

    public string Email { get; set; }

    public bool? Enabled { get; set; }

    public bool? Interrupted { get; set; }

    public int? ApiVersion { get; set; }

    public string AuthToken { get; set; }

    public WebhookSendType? SendType { get; set; }

    public List<WebhookEvent> Events { get; set; }
}
