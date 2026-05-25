using Codout.Apis.Asaas.Models.Webhook;
using Codout.Apis.Asaas.Models.Webhook.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para WebhookManager (B-32).
/// Schemas verificados via MCP em 2026-05-24.
/// Schema oficial confirma todos os 110+ valores do enum WebhookEvent.
/// </summary>
public class WebhookContractTests
{
    [Fact]
    public void WebhookResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("Webhook/webhook-response.json");

        var result = JsonContractAssert.DeserializeFixture<Webhook>(json);

        Assert.Equal("bbf67496-1379-4b6d-a348-fd5fa229f1c", result.Id);
        Assert.Equal("Name Example", result.Name);
        Assert.Equal("https://www.example.com/webhook/asaas", result.Url);
        Assert.Equal("john.doe@asaas.com.br", result.Email);
        Assert.True(result.Enabled);
        Assert.False(result.Interrupted);
        Assert.Equal(3, result.ApiVersion);
        Assert.True(result.HasAuthToken);
        Assert.Equal(WebhookSendType.SEQUENTIALLY, result.SendType);
        Assert.Equal(0, result.PenalizedRequestsCount);
        Assert.Equal(2, result.Events.Count);
        Assert.Contains(WebhookEvent.PAYMENT_RECEIVED, result.Events);
        Assert.Contains(WebhookEvent.PAYMENT_CONFIRMED, result.Events);
    }

    [Fact]
    public void SendType_BothValuesDeserialize()
    {
        foreach (var sendType in new[] { "SEQUENTIALLY", "NON_SEQUENTIALLY" })
        {
            var json = $"{{\"id\":\"x\",\"sendType\":\"{sendType}\"}}";
            var result = JsonContractAssert.DeserializeFixture<Webhook>(json);
            Assert.Equal(sendType, result.SendType.ToString());
        }
    }

    [Fact]
    public void WebhookEvent_SamplePaymentEventsDeserialize()
    {
        // Sample crítico: garantir que os eventos mais comuns deserializam.
        // Lista completa (110+ valores) e confirmada em CONFORMANCE.
        foreach (var ev in new[] {
            "PAYMENT_CREATED", "PAYMENT_RECEIVED", "PAYMENT_CONFIRMED",
            "PAYMENT_OVERDUE", "PAYMENT_REFUNDED", "PAYMENT_CHARGEBACK_REQUESTED",
            "INVOICE_AUTHORIZED", "TRANSFER_DONE", "SUBSCRIPTION_DELETED",
            "PIX_AUTOMATIC_RECURRING_AUTHORIZATION_ACTIVATED" })
        {
            var json = $"{{\"id\":\"x\",\"events\":[\"{ev}\"]}}";
            var result = JsonContractAssert.DeserializeFixture<Webhook>(json);
            Assert.Single(result.Events);
            Assert.Equal(ev, result.Events[0].ToString());
        }
    }
}
