using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Payment;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class PaymentManagerTests : ManagerTestBase<PaymentManager>
{
    protected override PaymentManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestablePaymentManager(settings, handler);

    #region Create

    [Fact]
    public async Task Create_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"pay_123\",\"customer\":\"cus_1\",\"value\":100.00}");
        var request = new CreatePaymentRequest
        {
            CustomerId = "cus_1",
            BillingType = BillingType.BOLETO,
            Value = 100.00m,
            DueDate = new DateTime(2026, 3, 15)
        };

        var result = await Manager.Create(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/payments");
    }

    [Fact]
    public async Task Create_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"pay_123\",\"customer\":\"cus_1\",\"value\":100.00,\"status\":\"PENDING\",\"description\":\"Test payment\"}");
        var request = new CreatePaymentRequest
        {
            CustomerId = "cus_1",
            BillingType = BillingType.BOLETO,
            Value = 100.00m
        };

        var result = await Manager.Create(request);

        Assert.True(result.WasSucessfull());
        Assert.Equal("pay_123", result.Data.Id);
        Assert.Equal("cus_1", result.Data.CustomerId);
        Assert.Equal(100.00m, result.Data.Value);
    }

    [Fact]
    public async Task Create_WhenApiReturnsError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.BadRequest);
        var request = new CreatePaymentRequest { CustomerId = "cus_1" };

        var result = await Manager.Create(request);

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotEmpty(result.Errors);
    }

    #endregion

    #region Find

    [Fact]
    public async Task Find_SendsGetToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"pay_456\",\"customer\":\"cus_1\",\"value\":50.00}");

        var result = await Manager.Find("pay_456");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/payments/pay_456");
    }

    [Fact]
    public async Task Find_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"pay_456\",\"customer\":\"cus_1\",\"value\":50.00,\"deleted\":false}");

        var result = await Manager.Find("pay_456");

        Assert.True(result.WasSucessfull());
        Assert.Equal("pay_456", result.Data.Id);
        Assert.Equal(50.00m, result.Data.Value);
        Assert.False(result.Data.Deleted);
    }

    #endregion

    #region List

    [Fact]
    public async Task List_SendsGetToCorrectUrl()
    {
        SetupListResponse<Payment>("[{\"id\":\"pay_1\",\"value\":100.00}]");

        var result = await Manager.List(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/payments");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    [Fact]
    public async Task List_DeserializesListResponseCorrectly()
    {
        SetupListResponse<Payment>("[{\"id\":\"pay_1\",\"value\":100.00},{\"id\":\"pay_2\",\"value\":200.00}]", totalCount: 2);

        var result = await Manager.List(0, 10);

        Assert.True(result.WasSucessfull());
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("pay_1", result.Data[0].Id);
        Assert.Equal("pay_2", result.Data[1].Id);
    }

    [Fact]
    public async Task List_WithFilter_IncludesFilterParametersInUrl()
    {
        SetupListResponse<Payment>("[{\"id\":\"pay_1\"}]");
        var filter = new PaymentListFilter { CustomerId = "cus_1", ExternalReference = "ref_123" };

        var result = await Manager.List(0, 10, filter);

        AssertRequestUrlContains("customer=cus_1");
        AssertRequestUrlContains("externalReference=ref_123");
    }

    #endregion

    #region Update

    [Fact]
    public async Task Update_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"pay_123\",\"value\":150.00}");
        var request = new UpdatePaymentRequest { Value = 150.00m };

        var result = await Manager.Update("pay_123", request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/payments/pay_123");
    }

    [Fact]
    public async Task Update_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"pay_123\",\"value\":150.00,\"description\":\"Updated payment\"}");
        var request = new UpdatePaymentRequest { Value = 150.00m, Description = "Updated payment" };

        var result = await Manager.Update("pay_123", request);

        Assert.True(result.WasSucessfull());
        Assert.Equal("pay_123", result.Data.Id);
        Assert.Equal(150.00m, result.Data.Value);
        Assert.Equal("Updated payment", result.Data.Description);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_SendsDeleteToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"pay_123\",\"deleted\":true}");

        var result = await Manager.Delete("pay_123");

        AssertRequestMethod(HttpMethod.Delete);
        AssertRequestUrl("/v3/payments/pay_123");
    }

    [Fact]
    public async Task Delete_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"pay_123\",\"deleted\":true}");

        var result = await Manager.Delete("pay_123");

        Assert.True(result.WasSucessfull());
        Assert.Equal("pay_123", result.Data.Id);
        Assert.True(result.Data.Deleted);
    }

    #endregion

    #region Restore

    [Fact]
    public async Task Restore_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"pay_123\",\"deleted\":false}");

        var result = await Manager.Restore("pay_123");

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/payments/pay_123/restore");
    }

    [Fact]
    public async Task Restore_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"pay_123\",\"value\":100.00,\"deleted\":false}");

        var result = await Manager.Restore("pay_123");

        Assert.True(result.WasSucessfull());
        Assert.Equal("pay_123", result.Data.Id);
        Assert.False(result.Data.Deleted);
    }

    #endregion

    #region Refund

    [Fact]
    public async Task Refund_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"pay_123\",\"status\":\"REFUNDED\"}");

        var result = await Manager.Refund("pay_123");

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/payments/pay_123/refund");
    }

    [Fact]
    public async Task Refund_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"pay_123\",\"status\":\"REFUNDED\",\"value\":100.00}");

        var result = await Manager.Refund("pay_123");

        Assert.True(result.WasSucessfull());
        Assert.Equal("pay_123", result.Data.Id);
    }

    #endregion

    #region ReceiveInCash

    [Fact]
    public async Task ReceiveInCash_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"pay_123\",\"status\":\"RECEIVED_IN_CASH\"}");

        var result = await Manager.ReceiveInCash("pay_123", new DateTime(2026, 3, 15), 100.00m, true);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/payments/pay_123/receiveInCash");
    }

    [Fact]
    public async Task ReceiveInCash_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"pay_123\",\"status\":\"RECEIVED_IN_CASH\",\"value\":100.00}");

        var result = await Manager.ReceiveInCash("pay_123", new DateTime(2026, 3, 15), 100.00m, false);

        Assert.True(result.WasSucessfull());
        Assert.Equal("pay_123", result.Data.Id);
    }

    #endregion

    #region GetBankSlipBarCode

    [Fact]
    public async Task GetBankSlipBarCode_SendsGetToCorrectUrl()
    {
        SetupOkResponse("{\"identificationField\":\"12345.67890\",\"nossoNumero\":\"1234567\",\"barCode\":\"12345678901234567890123456789012345678901234\"}");

        var result = await Manager.GetBankSlipBarCode("pay_123");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/payments/pay_123/identificationField");
    }

    [Fact]
    public async Task GetBankSlipBarCode_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"identificationField\":\"12345.67890\",\"nossoNumero\":\"1234567\",\"barCode\":\"12345678901234567890123456789012345678901234\"}");

        var result = await Manager.GetBankSlipBarCode("pay_123");

        Assert.True(result.WasSucessfull());
        Assert.Equal("12345.67890", result.Data.IdentificationField);
        Assert.Equal("1234567", result.Data.NossoNumero);
        Assert.Equal("12345678901234567890123456789012345678901234", result.Data.BarCode);
    }

    #endregion

    #region GetPixQrCode

    [Fact]
    public async Task GetPixQrCode_SendsGetToCorrectUrl()
    {
        SetupOkResponse("{\"encodedImage\":\"base64data\",\"payload\":\"pixpayload\",\"expirationDate\":\"2026-03-15T00:00:00\"}");

        var result = await Manager.GetPixQrCode("pay_123");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/payments/pay_123/pixQrCode");
    }

    [Fact]
    public async Task GetPixQrCode_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"encodedImage\":\"base64data\",\"payload\":\"pixpayload\",\"expirationDate\":\"2026-03-15T00:00:00\"}");

        var result = await Manager.GetPixQrCode("pay_123");

        Assert.True(result.WasSucessfull());
        Assert.Equal("base64data", result.Data.EncodedImage);
        Assert.Equal("pixpayload", result.Data.Payload);
    }

    #endregion

    #region UndoReceivedInCash

    [Fact]
    public async Task UndoReceivedInCash_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"pay_123\",\"status\":\"PENDING\"}");

        var result = await Manager.UndoReceivedInCash("pay_123");

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/payments/pay_123/undoReceivedInCash");
    }

    [Fact]
    public async Task UndoReceivedInCash_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"pay_123\",\"status\":\"PENDING\",\"value\":100.00}");

        var result = await Manager.UndoReceivedInCash("pay_123");

        Assert.True(result.WasSucessfull());
        Assert.Equal("pay_123", result.Data.Id);
    }

    #endregion
}
