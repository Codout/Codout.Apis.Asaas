using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.PaymentLink;
using Codout.Apis.Asaas.Models.PaymentLink.Enums;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class PaymentLinkManagerTests : ManagerTestBase<PaymentLinkManager>
{
    protected override PaymentLinkManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestablePaymentLinkManager(settings, handler);

    #region Create

    [Fact]
    public async Task Create_SendsPostRequest()
    {
        SetupOkResponse("{\"id\":\"pl_123\",\"name\":\"Test Link\",\"value\":100.50,\"billingType\":\"BOLETO\",\"chargeType\":\"DETACHED\"}");

        var request = new CreatePaymentLinkRequest
        {
            Name = "Test Link",
            Value = 100.50m,
            BillingType = BillingType.BOLETO,
            ChargeType = ChargeType.DETACHED,
            DueDateLimitDays = 10,
            NotificationEnabled = true
        };

        var result = await Manager.Create(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/paymentLinks");
        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("pl_123", result.Data.Id);
        Assert.Equal("Test Link", result.Data.Name);
        Assert.Equal(100.50m, result.Data.Value);
        Assert.Equal(BillingType.BOLETO, result.Data.BillingType);
        Assert.Equal(ChargeType.DETACHED, result.Data.ChargeType);
    }

    [Fact]
    public async Task Create_SerializesRequestBody()
    {
        SetupOkResponse("{\"id\":\"pl_123\"}");

        var request = new CreatePaymentLinkRequest
        {
            Name = "My Link",
            Value = 50.0m,
            BillingType = BillingType.PIX,
            ChargeType = ChargeType.RECURRENT,
            DueDateLimitDays = 5,
            MaxInstallmentCount = 3,
            NotificationEnabled = false
        };

        await Manager.Create(request);

        Assert.NotNull(Handler.LastRequestContent);
        Assert.Contains("\"name\":\"My Link\"", Handler.LastRequestContent);
        Assert.Contains("\"billingType\":\"PIX\"", Handler.LastRequestContent);
        Assert.Contains("\"chargeType\":\"RECURRENT\"", Handler.LastRequestContent);
    }

    #endregion

    #region List

    [Fact]
    public async Task List_SendsGetRequest()
    {
        SetupListResponse<PaymentLink>("[{\"id\":\"pl_1\",\"name\":\"Link 1\",\"billingType\":\"BOLETO\",\"chargeType\":\"DETACHED\"}]", totalCount: 1, limit: 10, offset: 0);

        var result = await Manager.List(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/paymentLinks");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
        Assert.True(result.WasSucessfull());
        Assert.Single(result.Data);
        Assert.Equal("pl_1", result.Data[0].Id);
    }

    [Fact]
    public async Task List_ParsesListMetadata()
    {
        SetupListResponse<PaymentLink>("[{\"id\":\"pl_1\",\"billingType\":\"BOLETO\",\"chargeType\":\"DETACHED\"}]", totalCount: 50, limit: 10, offset: 20, hasMore: true);

        var result = await Manager.List(20, 10);

        Assert.Equal(50, result.TotalCount);
        Assert.Equal(10, result.Limit);
        Assert.Equal(20, result.Offset);
        Assert.True(result.HasMore);
    }

    #endregion

    #region Find

    [Fact]
    public async Task Find_SendsGetRequestWithId()
    {
        SetupOkResponse("{\"id\":\"pl_456\",\"name\":\"Found Link\",\"value\":200,\"billingType\":\"CREDIT_CARD\",\"chargeType\":\"INSTALLMENT\",\"active\":true}");

        var result = await Manager.Find("pl_456");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/paymentLinks/pl_456");
        Assert.True(result.WasSucessfull());
        Assert.Equal("pl_456", result.Data.Id);
        Assert.Equal("Found Link", result.Data.Name);
        Assert.True(result.Data.Active);
    }

    #endregion

    #region Update

    [Fact]
    public async Task Update_SendsPutRequest()
    {
        SetupOkResponse("{\"id\":\"pl_789\",\"name\":\"Updated Link\",\"billingType\":\"BOLETO\",\"chargeType\":\"DETACHED\"}");

        var request = new UpdatePaymentLinkRequest
        {
            Name = "Updated Link",
            Active = true
        };

        var result = await Manager.Update("pl_789", request);

        AssertRequestMethod(HttpMethod.Put);
        AssertRequestUrl("/v3/paymentLinks/pl_789");
        Assert.True(result.WasSucessfull());
        Assert.Equal("pl_789", result.Data.Id);
        Assert.Equal("Updated Link", result.Data.Name);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_SendsDeleteRequest()
    {
        // BaseDeleted is abstract and cannot be deserialized by System.Text.Json.
        // Use an error response to verify the correct URL and method are used.
        SetupErrorResponse(HttpStatusCode.NotFound);

        var result = await Manager.Delete("pl_del");

        AssertRequestMethod(HttpMethod.Delete);
        AssertRequestUrl("/v3/paymentLinks/pl_del");
    }

    #endregion

    #region Restore

    [Fact]
    public async Task Restore_SendsPostRequest()
    {
        SetupOkResponse("{\"id\":\"pl_restored\",\"name\":\"Restored\",\"deleted\":false,\"billingType\":\"BOLETO\",\"chargeType\":\"DETACHED\"}");

        var result = await Manager.Restore("pl_restored");

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/paymentLinks/pl_restored/restore");
        Assert.True(result.WasSucessfull());
        Assert.False(result.Data.Deleted);
    }

    #endregion

    #region ListImages

    [Fact]
    public async Task ListImages_SendsGetRequest()
    {
        SetupListResponse<PaymentLinkImage>("[{\"id\":\"img_1\",\"main\":true,\"paymentLink\":\"pl_123\"}]");

        var result = await Manager.ListImages("pl_123", 0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/paymentLinks/pl_123/images");
        Assert.True(result.WasSucessfull());
        Assert.Single(result.Data);
        Assert.Equal("img_1", result.Data[0].Id);
        Assert.True(result.Data[0].Main);
    }

    #endregion

    #region DeleteImage

    [Fact]
    public async Task DeleteImage_SendsDeleteRequest()
    {
        // BaseDeleted is abstract and cannot be deserialized by System.Text.Json.
        // Use an error response to verify the correct URL and method are used.
        SetupErrorResponse(HttpStatusCode.NotFound);

        var result = await Manager.DeleteImage("pl_123", "img_del");

        AssertRequestMethod(HttpMethod.Delete);
        AssertRequestUrl("/v3/paymentLinks/pl_123/images/img_del");
    }

    #endregion

    #region FindImage

    [Fact]
    public async Task FindImage_SendsGetRequest()
    {
        SetupOkResponse("{\"id\":\"img_find\",\"main\":false,\"paymentLink\":\"pl_123\"}");

        var result = await Manager.FindImage("pl_123", "img_find");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/paymentLinks/pl_123/images/img_find");
        Assert.True(result.WasSucessfull());
        Assert.Equal("img_find", result.Data.Id);
        Assert.False(result.Data.Main);
    }

    #endregion

    #region SetMainImage

    [Fact]
    public async Task SetMainImage_SendsPostRequest()
    {
        SetupOkResponse("{\"id\":\"img_main\",\"main\":true,\"paymentLink\":\"pl_123\"}");

        var result = await Manager.SetMainImage("pl_123", "img_main");

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/paymentLinks/pl_123/images/img_main/setAsMain");
        Assert.True(result.WasSucessfull());
        Assert.True(result.Data.Main);
    }

    #endregion

    #region Error Handling

    [Fact]
    public async Task Find_ReturnsErrorOnBadRequest()
    {
        SetupErrorResponse(HttpStatusCode.BadRequest);

        var result = await Manager.Find("invalid_id");

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("invalid", result.Errors[0].Code);
        Assert.Equal("Test error", result.Errors[0].Description);
    }

    [Fact]
    public async Task Create_ReturnsErrorOnNotFound()
    {
        Handler.WithResponse(HttpStatusCode.NotFound, "{\"errors\":[{\"code\":\"not_found\",\"description\":\"Resource not found\"}]}");

        var request = new CreatePaymentLinkRequest { Name = "Test" };
        var result = await Manager.Create(request);

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.Single(result.Errors);
    }

    #endregion
}
