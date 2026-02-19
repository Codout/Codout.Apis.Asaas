using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.CreditCard;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class CreditCardManagerTests : ManagerTestBase<CreditCardManager>
{
    protected override CreditCardManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableCreditCardManager(settings, handler);

    // ── TokenizeCreditCard ──────────────────────────────────────────

    [Fact]
    public async Task TokenizeCreditCard_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"creditCardNumber\":\"1234\",\"creditCardBrand\":\"VISA\",\"creditCardToken\":\"tok_abc123\"}");

        var request = new TokenizeCreditCardRequest
        {
            Customer = "cust_123",
            RemoteIp = "192.168.1.1"
        };

        var result = await Manager.TokenizeCreditCard(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/creditCard/tokenizeCreditCard");
    }

    [Fact]
    public async Task TokenizeCreditCard_DeserializesResponse()
    {
        SetupOkResponse("{\"creditCardNumber\":\"4444\",\"creditCardBrand\":\"MASTERCARD\",\"creditCardToken\":\"tok_xyz789\"}");

        var request = new TokenizeCreditCardRequest
        {
            Customer = "cust_456",
            RemoteIp = "10.0.0.1"
        };

        var result = await Manager.TokenizeCreditCard(request);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("4444", result.Data.Number);
        Assert.Equal("MASTERCARD", result.Data.Brand);
        Assert.Equal("tok_xyz789", result.Data.Token);
    }

    [Fact]
    public async Task TokenizeCreditCard_WithFullRequest_DeserializesResponse()
    {
        SetupOkResponse("{\"creditCardNumber\":\"1111\",\"creditCardBrand\":\"VISA\",\"creditCardToken\":\"tok_full_test\"}");

        var request = new TokenizeCreditCardRequest
        {
            Customer = "cust_789",
            RemoteIp = "172.16.0.1"
        };

        var result = await Manager.TokenizeCreditCard(request);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("1111", result.Data.Number);
        Assert.Equal("VISA", result.Data.Brand);
        Assert.Equal("tok_full_test", result.Data.Token);
    }

    // ── Error handling ──────────────────────────────────────────────

    [Fact]
    public async Task TokenizeCreditCard_OnBadRequest_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.BadRequest);

        var request = new TokenizeCreditCardRequest
        {
            Customer = "cust_invalid",
            RemoteIp = "0.0.0.0"
        };

        var result = await Manager.TokenizeCreditCard(request);

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("invalid", result.Errors[0].Code);
        Assert.Equal("Test error", result.Errors[0].Description);
    }

    [Fact]
    public async Task TokenizeCreditCard_OnUnauthorized_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.Unauthorized);

        var request = new TokenizeCreditCardRequest
        {
            Customer = "cust_123",
            RemoteIp = "192.168.1.1"
        };

        var result = await Manager.TokenizeCreditCard(request);

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task TokenizeCreditCard_OnInternalServerError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.InternalServerError);

        var request = new TokenizeCreditCardRequest
        {
            Customer = "cust_123",
            RemoteIp = "192.168.1.1"
        };

        var result = await Manager.TokenizeCreditCard(request);

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        Assert.NotEmpty(result.Errors);
    }
}
