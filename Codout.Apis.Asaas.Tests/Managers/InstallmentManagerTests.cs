using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.Installment;
using Codout.Apis.Asaas.Models.Payment;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class InstallmentManagerTests : ManagerTestBase<InstallmentManager>
{
    protected override InstallmentManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableInstallmentManager(settings, handler);

    #region Find

    [Fact]
    public async Task Find_SendsGetToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"inst_123\",\"value\":500.00,\"installmentCount\":5}");

        var result = await Manager.Find("inst_123");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/installments/inst_123");
    }

    [Fact]
    public async Task Find_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"inst_123\",\"value\":500.00,\"netValue\":480.00,\"paymentValue\":100.00,\"installmentCount\":5,\"description\":\"Monthly installment\",\"customer\":\"cus_1\",\"deleted\":false}");

        var result = await Manager.Find("inst_123");

        Assert.True(result.WasSucessfull());
        Assert.Equal("inst_123", result.Data.Id);
        Assert.Equal(500.00m, result.Data.Value);
        Assert.Equal(480.00m, result.Data.NetValue);
        Assert.Equal(100.00m, result.Data.PaymentValue);
        Assert.Equal(5, result.Data.InstallmentCount);
        Assert.Equal("Monthly installment", result.Data.Description);
        Assert.Equal("cus_1", result.Data.CustomerId);
        Assert.False(result.Data.Deleted);
    }

    [Fact]
    public async Task Find_WhenApiReturnsError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.NotFound);

        var result = await Manager.Find("inst_nonexistent");

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.NotEmpty(result.Errors);
    }

    #endregion

    #region List

    [Fact]
    public async Task List_SendsGetToCorrectUrl()
    {
        SetupListResponse<Installment>("[{\"id\":\"inst_1\",\"value\":500.00}]");

        var result = await Manager.List(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/installments");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    [Fact]
    public async Task List_DeserializesListResponseCorrectly()
    {
        SetupListResponse<Installment>("[{\"id\":\"inst_1\",\"value\":500.00},{\"id\":\"inst_2\",\"value\":300.00}]", totalCount: 2);

        var result = await Manager.List(0, 10);

        Assert.True(result.WasSucessfull());
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("inst_1", result.Data[0].Id);
        Assert.Equal("inst_2", result.Data[1].Id);
    }

    [Fact]
    public async Task List_WithPagination_IncludesOffsetAndLimit()
    {
        SetupListResponse<Installment>("[]", totalCount: 0, limit: 20, offset: 5);

        var result = await Manager.List(5, 20);

        AssertRequestUrlContains("offset=5");
        AssertRequestUrlContains("limit=20");
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_SendsDeleteToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"inst_123\",\"deleted\":true}");

        var result = await Manager.Delete("inst_123");

        AssertRequestMethod(HttpMethod.Delete);
        AssertRequestUrl("/v3/installments/inst_123");
    }

    [Fact]
    public async Task Delete_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"inst_123\",\"deleted\":true}");

        var result = await Manager.Delete("inst_123");

        Assert.True(result.WasSucessfull());
        Assert.Equal("inst_123", result.Data.Id);
        Assert.True(result.Data.Deleted);
    }

    #endregion

    #region Refund

    [Fact]
    public async Task Refund_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"inst_123\",\"value\":500.00}");

        var result = await Manager.Refund("inst_123");

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/installments/inst_123/refund");
    }

    [Fact]
    public async Task Refund_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"inst_123\",\"value\":500.00,\"installmentCount\":5}");

        var result = await Manager.Refund("inst_123");

        Assert.True(result.WasSucessfull());
        Assert.Equal("inst_123", result.Data.Id);
        Assert.Equal(500.00m, result.Data.Value);
    }

    #endregion

    #region ListPaymentBook

    [Fact]
    public async Task ListPaymentBook_SendsGetToCorrectUrl()
    {
        SetupListResponse<Payment>("[{\"id\":\"pay_1\",\"value\":100.00}]");

        var result = await Manager.ListPaymentBook("inst_123", 0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/installments/inst_123/paymentBook");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    [Fact]
    public async Task ListPaymentBook_DeserializesResponseCorrectly()
    {
        SetupListResponse<Payment>("[{\"id\":\"pay_1\",\"value\":100.00},{\"id\":\"pay_2\",\"value\":100.00},{\"id\":\"pay_3\",\"value\":100.00}]", totalCount: 3);

        var result = await Manager.ListPaymentBook("inst_123", 0, 10);

        Assert.True(result.WasSucessfull());
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Data.Count);
        Assert.Equal("pay_1", result.Data[0].Id);
        Assert.Equal("pay_2", result.Data[1].Id);
        Assert.Equal("pay_3", result.Data[2].Id);
    }

    #endregion
}
