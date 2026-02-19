using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.Webhook;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class WebhookManagerTests : ManagerTestBase<WebhookManager>
{
    protected override WebhookManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableWebhookManager(settings, handler);

    // ── FindPaymentWebhook ──────────────────────────────────────────

    [Fact]
    public async Task FindPaymentWebhook_SendsGetRequest()
    {
        SetupOkResponse("{\"url\":\"https://example.com\",\"enabled\":true}");

        var result = await Manager.FindPaymentWebhook();

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/webhook");
    }

    [Fact]
    public async Task FindPaymentWebhook_DeserializesResponse()
    {
        SetupOkResponse("{\"url\":\"https://example.com\",\"email\":\"test@test.com\",\"apiVersion\":3,\"enabled\":true,\"interrupted\":false,\"authToken\":\"tok123\"}");

        var result = await Manager.FindPaymentWebhook();

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("https://example.com", result.Data.Url);
        Assert.Equal("test@test.com", result.Data.Email);
        Assert.Equal(3, result.Data.ApiVersion);
        Assert.True(result.Data.Enabled);
        Assert.False(result.Data.Interrupted);
        Assert.Equal("tok123", result.Data.AuthToken);
    }

    // ── CreateOrUpdatePaymentWebhook ────────────────────────────────

    [Fact]
    public async Task CreateOrUpdatePaymentWebhook_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"url\":\"https://example.com\",\"enabled\":true}");

        var request = new WebhookRequest
        {
            Url = "https://example.com",
            Email = "test@test.com",
            ApiVersion = 3,
            Enabled = true
        };

        var result = await Manager.CreateOrUpdatePaymentWebhook(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/webhook");
    }

    [Fact]
    public async Task CreateOrUpdatePaymentWebhook_DeserializesResponse()
    {
        SetupOkResponse("{\"url\":\"https://example.com\",\"enabled\":true,\"apiVersion\":3}");

        var request = new WebhookRequest { Url = "https://example.com", Enabled = true, ApiVersion = 3 };

        var result = await Manager.CreateOrUpdatePaymentWebhook(request);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("https://example.com", result.Data.Url);
        Assert.True(result.Data.Enabled);
    }

    // ── FindInvoiceWebhook ──────────────────────────────────────────

    [Fact]
    public async Task FindInvoiceWebhook_SendsGetToCorrectUrl()
    {
        SetupOkResponse("{\"url\":\"https://invoice.example.com\",\"enabled\":true}");

        var result = await Manager.FindInvoiceWebhook();

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/webhook/invoice");
    }

    [Fact]
    public async Task FindInvoiceWebhook_DeserializesResponse()
    {
        SetupOkResponse("{\"url\":\"https://invoice.example.com\",\"enabled\":false}");

        var result = await Manager.FindInvoiceWebhook();

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("https://invoice.example.com", result.Data.Url);
        Assert.False(result.Data.Enabled);
    }

    // ── CreateOrUpdateInvoiceWebhook ────────────────────────────────

    [Fact]
    public async Task CreateOrUpdateInvoiceWebhook_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"url\":\"https://invoice.example.com\",\"enabled\":true}");

        var request = new WebhookRequest { Url = "https://invoice.example.com", Enabled = true };

        var result = await Manager.CreateOrUpdateInvoiceWebhook(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/webhook/invoice");
    }

    // ── FindMobilePhoneRechargeWebhook ──────────────────────────────

    [Fact]
    public async Task FindMobilePhoneRechargeWebhook_SendsGetToCorrectUrl()
    {
        SetupOkResponse("{\"url\":\"https://mobile.example.com\",\"enabled\":true}");

        var result = await Manager.FindMobilePhoneRechargeWebhook();

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/webhook/mobilePhoneRecharge");
    }

    [Fact]
    public async Task FindMobilePhoneRechargeWebhook_DeserializesResponse()
    {
        SetupOkResponse("{\"url\":\"https://mobile.example.com\",\"enabled\":true,\"apiVersion\":3}");

        var result = await Manager.FindMobilePhoneRechargeWebhook();

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("https://mobile.example.com", result.Data.Url);
    }

    // ── CreateOrUpdateMobilePhoneRechargeWebhook ────────────────────

    [Fact]
    public async Task CreateOrUpdateMobilePhoneRechargeWebhook_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"url\":\"https://mobile.example.com\",\"enabled\":true}");

        var request = new WebhookRequest { Url = "https://mobile.example.com", Enabled = true };

        var result = await Manager.CreateOrUpdateMobilePhoneRechargeWebhook(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/webhook/mobilePhoneRecharge");
    }

    // ── Error handling ──────────────────────────────────────────────

    [Fact]
    public async Task FindPaymentWebhook_OnError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.BadRequest);

        var result = await Manager.FindPaymentWebhook();

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("invalid", result.Errors[0].Code);
        Assert.Equal("Test error", result.Errors[0].Description);
    }

    [Fact]
    public async Task CreateOrUpdatePaymentWebhook_OnError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.InternalServerError);

        var request = new WebhookRequest { Url = "https://example.com" };
        var result = await Manager.CreateOrUpdatePaymentWebhook(request);

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        Assert.NotEmpty(result.Errors);
    }
}
