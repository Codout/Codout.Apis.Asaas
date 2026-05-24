using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.Webhook;
using Codout.Apis.Asaas.Models.Webhook.Enums;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class WebhookManagerTests : ManagerTestBase<WebhookManager>
{
    protected override WebhookManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableWebhookManager(settings, handler);

    #region Create

    [Fact]
    public async Task Create_SendsPostToWebhooksRoute()
    {
        SetupOkResponse("{\"id\":\"wh_new\",\"name\":\"My webhook\",\"url\":\"https://example.com\",\"enabled\":true}");
        var request = new CreateWebhookRequest
        {
            Name = "My webhook",
            Url = "https://example.com",
            Email = "ops@example.com",
            Enabled = true,
            Interrupted = false,
            ApiVersion = 3,
            AuthToken = "whsec_Pxeh17yy3LQbLVpnzz6I1chB7mtzYk5F7pg8bRR80pE",
            SendType = WebhookSendType.SEQUENTIALLY,
            Events = [WebhookEvent.PAYMENT_CONFIRMED, WebhookEvent.PAYMENT_RECEIVED]
        };

        var result = await Manager.Create(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/webhooks");
        Assert.True(result.WasSuccessful());
        Assert.Equal("wh_new", result.Data.Id);
    }

    [Fact]
    public async Task Create_SerializesAllFields()
    {
        SetupOkResponse("{\"id\":\"wh_new\"}");
        var request = new CreateWebhookRequest
        {
            Name = "Webhook A",
            Url = "https://example.com/hook",
            Email = "ops@example.com",
            Enabled = true,
            Interrupted = false,
            ApiVersion = 3,
            AuthToken = "whsec_abcdefghijklmnopqrstuvwxyz123456",
            SendType = WebhookSendType.NON_SEQUENTIALLY,
            Events = [WebhookEvent.PAYMENT_CREATED, WebhookEvent.PIX_AUTOMATIC_RECURRING_AUTHORIZATION_CREATED]
        };

        await Manager.Create(request);

        Assert.NotNull(Handler.LastRequestContent);
        Assert.Contains("\"name\":\"Webhook A\"", Handler.LastRequestContent);
        Assert.Contains("\"url\":\"https://example.com/hook\"", Handler.LastRequestContent);
        Assert.Contains("\"sendType\":\"NON_SEQUENTIALLY\"", Handler.LastRequestContent);
        Assert.Contains("\"PAYMENT_CREATED\"", Handler.LastRequestContent);
        Assert.Contains("\"PIX_AUTOMATIC_RECURRING_AUTHORIZATION_CREATED\"", Handler.LastRequestContent);
    }

    #endregion

    #region List

    [Fact]
    public async Task List_SendsGetToWebhooksRoute()
    {
        SetupListResponse<Webhook>("[{\"id\":\"wh_1\",\"name\":\"A\"}]");

        var result = await Manager.List(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/webhooks");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    [Fact]
    public async Task List_WithFilter_IncludesFilterParameters()
    {
        SetupListResponse<Webhook>("[]");
        var filter = new WebhookListFilter { Name = "Payments", Enabled = true };

        await Manager.List(0, 10, filter);

        AssertRequestUrlContains("name=Payments");
        AssertRequestUrlContains("enabled=true");
    }

    #endregion

    #region Find

    [Fact]
    public async Task Find_SendsGetToWebhooksId()
    {
        SetupOkResponse("{\"id\":\"wh_42\",\"name\":\"Found\",\"url\":\"https://example.com\",\"events\":[\"PAYMENT_RECEIVED\"]}");

        var result = await Manager.Find("wh_42");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/webhooks/wh_42");
        Assert.True(result.WasSuccessful());
        Assert.Equal("wh_42", result.Data.Id);
        Assert.Equal("Found", result.Data.Name);
        Assert.Single(result.Data.Events);
        Assert.Equal(WebhookEvent.PAYMENT_RECEIVED, result.Data.Events[0]);
    }

    #endregion

    #region Update

    [Fact]
    public async Task Update_SendsPutToWebhooksId()
    {
        SetupOkResponse("{\"id\":\"wh_42\",\"enabled\":false}");
        var request = new UpdateWebhookRequest { Enabled = false };

        var result = await Manager.Update("wh_42", request);

        AssertRequestMethod(HttpMethod.Put);
        AssertRequestUrl("/v3/webhooks/wh_42");
        Assert.True(result.WasSuccessful());
        Assert.False(result.Data.Enabled);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_SendsDeleteToWebhooksId()
    {
        SetupOkResponse("{\"id\":\"wh_42\",\"deleted\":true}");

        var result = await Manager.Delete("wh_42");

        AssertRequestMethod(HttpMethod.Delete);
        AssertRequestUrl("/v3/webhooks/wh_42");
        Assert.True(result.WasSuccessful());
        Assert.True(result.Data.Deleted);
    }

    #endregion

    #region RemoveBackoff

    [Fact]
    public async Task RemoveBackoff_SendsPostToRemoveBackoffRoute()
    {
        SetupOkResponse("{\"id\":\"wh_42\",\"interrupted\":false}");

        var result = await Manager.RemoveBackoff("wh_42");

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/webhooks/wh_42/removeBackoff");
        Assert.True(result.WasSuccessful());
    }

    #endregion

    #region Error Handling

    [Fact]
    public async Task Create_OnError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.BadRequest);
        var request = new CreateWebhookRequest { Url = "https://example.com" };

        var result = await Manager.Create(request);

        Assert.False(result.WasSuccessful());
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task Find_OnNotFound_ReturnsError()
    {
        SetupErrorResponse(HttpStatusCode.NotFound);

        var result = await Manager.Find("wh_unknown");

        Assert.False(result.WasSuccessful());
    }

    #endregion
}
