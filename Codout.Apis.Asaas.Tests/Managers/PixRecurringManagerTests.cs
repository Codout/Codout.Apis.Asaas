using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.PixRecurring;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class PixRecurringManagerTests : ManagerTestBase<PixRecurringManager>
{
    protected override PixRecurringManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestablePixRecurringManager(settings, handler);

    [Fact]
    public async Task List_SendsGetToRecurringsRoute()
    {
        SetupListResponse<PixRecurringTransaction>("[]");

        var result = await Manager.List(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/pix/transactions/recurrings");
    }

    [Fact]
    public async Task Find_SendsGetToRecurringIdRoute()
    {
        SetupOkResponse("{\"id\":\"rec_1\"}");

        var result = await Manager.Find("rec_1");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/pix/transactions/recurrings/rec_1");
    }

    [Fact]
    public async Task Cancel_SendsPostToCancelRoute()
    {
        SetupOkResponse("{\"id\":\"rec_1\",\"status\":\"CANCELLED\"}");

        var result = await Manager.Cancel("rec_1");

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/pix/transactions/recurrings/rec_1/cancel");
    }

    [Fact]
    public async Task ListItems_SendsGetToItemsRoute()
    {
        SetupListResponse<PixRecurringItem>("[]");

        var result = await Manager.ListItems("rec_1", 0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/pix/transactions/recurrings/rec_1/items");
    }

    [Fact]
    public async Task CancelItem_SendsPostToItemCancelRoute()
    {
        SetupOkResponse("{\"id\":\"item_1\",\"status\":\"CANCELLED\"}");

        var result = await Manager.CancelItem("item_1");

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/pix/transactions/recurrings/items/item_1/cancel");
    }

    [Fact]
    public async Task Find_OnNotFound_ReturnsError()
    {
        SetupErrorResponse(System.Net.HttpStatusCode.NotFound);

        var result = await Manager.Find("rec_unknown");

        Assert.False(result.WasSuccessful());
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task Cancel_OnError_ReturnsErrorResponse()
    {
        SetupErrorResponse(System.Net.HttpStatusCode.BadRequest);

        var result = await Manager.Cancel("rec_1");

        Assert.False(result.WasSuccessful());
    }
}
