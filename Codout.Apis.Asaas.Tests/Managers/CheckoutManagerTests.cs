using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.Checkout;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class CheckoutManagerTests : ManagerTestBase<CheckoutManager>
{
    protected override CheckoutManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableCheckoutManager(settings, handler);

    [Fact]
    public async Task Create_SendsPostToCheckoutsRoute()
    {
        SetupOkResponse("{\"id\":\"ck_1\",\"status\":\"ACTIVE\",\"checkoutUrl\":\"https://asaas.com/c/ck_1\"}");
        var request = new CreateCheckoutRequest { Value = 100m };

        var result = await Manager.Create(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/checkouts");
        Assert.True(result.WasSucessfull());
        Assert.Equal("ck_1", result.Data.Id);
    }

    [Fact]
    public async Task Cancel_SendsPostToCheckoutCancelRoute()
    {
        SetupOkResponse("{\"id\":\"ck_1\",\"status\":\"CANCELED\"}");

        var result = await Manager.Cancel("ck_1");

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/checkouts/ck_1/cancel");
    }
}
