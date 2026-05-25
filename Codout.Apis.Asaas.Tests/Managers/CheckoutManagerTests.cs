using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.Checkout;
using Codout.Apis.Asaas.Models.Checkout.Enums;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class CheckoutManagerTests : ManagerTestBase<CheckoutManager>
{
    protected override CheckoutManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableCheckoutManager(settings, handler);

    [Fact]
    public async Task Create_SendsPostToCheckoutsRoute()
    {
        SetupOkResponse("{\"id\":\"ck_1\",\"status\":\"ACTIVE\",\"link\":\"https://sandbox.asaas.com/checkoutSession/show/ck_1\",\"billingTypes\":[\"CREDIT_CARD\"],\"chargeTypes\":[\"DETACHED\"]}");
        var request = new CreateCheckoutRequest
        {
            BillingTypes = [CheckoutBillingType.CREDIT_CARD],
            ChargeTypes = [CheckoutChargeType.DETACHED],
            Callback = new CheckoutCallback { SuccessUrl = "https://example.com/ok", CancelUrl = "https://example.com/cancel" },
            Items = [new CheckoutItem { Name = "Produto", Quantity = 1, Value = 100m, ImageBase64 = "..." }]
        };

        var result = await Manager.Create(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/checkouts");
        Assert.True(result.WasSuccessful());
        Assert.Equal("ck_1", result.Data.Id);
        Assert.Equal(CheckoutStatus.ACTIVE, result.Data.Status);
        Assert.Contains(CheckoutBillingType.CREDIT_CARD, result.Data.BillingTypes);
    }

    [Fact]
    public async Task Create_SerializesItemsAndCallback()
    {
        SetupOkResponse("{\"id\":\"ck_2\"}");
        var request = new CreateCheckoutRequest
        {
            BillingTypes = [CheckoutBillingType.PIX],
            ChargeTypes = [CheckoutChargeType.DETACHED],
            Callback = new CheckoutCallback { SuccessUrl = "https://example.com/ok", CancelUrl = "https://example.com/cancel" },
            Items = [new CheckoutItem { Name = "Servico", Quantity = 2, Value = 50m, ImageBase64 = "X" }]
        };

        await Manager.Create(request);

        Assert.NotNull(Handler.LastRequestContent);
        Assert.Contains("\"billingTypes\":[\"PIX\"]", Handler.LastRequestContent);
        Assert.Contains("\"successUrl\":\"https://example.com/ok\"", Handler.LastRequestContent);
        Assert.Contains("\"items\":[", Handler.LastRequestContent);
    }

    [Fact]
    public async Task Cancel_SendsPostToCheckoutCancelRoute()
    {
        SetupOkResponse("{\"id\":\"ck_1\",\"status\":\"CANCELED\"}");

        var result = await Manager.Cancel("ck_1");

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/checkouts/ck_1/cancel");
    }

    [Fact]
    public async Task Create_OnError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.BadRequest);
        var request = new CreateCheckoutRequest();

        var result = await Manager.Create(request);

        Assert.False(result.WasSuccessful());
        Assert.NotEmpty(result.Errors);
    }
}
