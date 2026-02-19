using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Invoice;
using Codout.Apis.Asaas.Models.Payment;
using Codout.Apis.Asaas.Models.Subscription;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class SubscriptionManagerTests : ManagerTestBase<SubscriptionManager>
{
    protected override SubscriptionManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableSubscriptionManager(settings, handler);

    #region Create

    [Fact]
    public async Task Create_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"sub_123\",\"customer\":\"cus_1\",\"value\":99.90}");
        var request = new CreateSubscriptionRequest
        {
            CustomerId = "cus_1",
            BillingType = BillingType.CREDIT_CARD,
            Value = 99.90m,
            NextDueDate = new DateTime(2026, 4, 1)
        };

        var result = await Manager.Create(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/subscriptions");
    }

    [Fact]
    public async Task Create_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"sub_123\",\"customer\":\"cus_1\",\"value\":99.90,\"status\":\"ACTIVE\",\"description\":\"Monthly plan\"}");
        var request = new CreateSubscriptionRequest
        {
            CustomerId = "cus_1",
            BillingType = BillingType.CREDIT_CARD,
            Value = 99.90m,
            NextDueDate = new DateTime(2026, 4, 1)
        };

        var result = await Manager.Create(request);

        Assert.True(result.WasSucessfull());
        Assert.Equal("sub_123", result.Data.Id);
        Assert.Equal("cus_1", result.Data.CustomerId);
        Assert.Equal(99.90m, result.Data.Value);
        Assert.Equal("Monthly plan", result.Data.Description);
    }

    [Fact]
    public async Task Create_WhenApiReturnsError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.BadRequest);
        var request = new CreateSubscriptionRequest { CustomerId = "cus_1" };

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
        SetupOkResponse("{\"id\":\"sub_456\",\"customer\":\"cus_1\",\"value\":49.90}");

        var result = await Manager.Find("sub_456");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/subscriptions/sub_456");
    }

    [Fact]
    public async Task Find_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"sub_456\",\"customer\":\"cus_1\",\"value\":49.90,\"deleted\":false}");

        var result = await Manager.Find("sub_456");

        Assert.True(result.WasSucessfull());
        Assert.Equal("sub_456", result.Data.Id);
        Assert.Equal("cus_1", result.Data.CustomerId);
        Assert.Equal(49.90m, result.Data.Value);
        Assert.False(result.Data.Deleted);
    }

    #endregion

    #region List

    [Fact]
    public async Task List_SendsGetToCorrectUrl()
    {
        SetupListResponse<Subscription>("[{\"id\":\"sub_1\",\"value\":99.90}]");

        var result = await Manager.List(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/subscriptions");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    [Fact]
    public async Task List_DeserializesListResponseCorrectly()
    {
        SetupListResponse<Subscription>("[{\"id\":\"sub_1\",\"value\":99.90},{\"id\":\"sub_2\",\"value\":49.90}]", totalCount: 2);

        var result = await Manager.List(0, 10);

        Assert.True(result.WasSucessfull());
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("sub_1", result.Data[0].Id);
        Assert.Equal("sub_2", result.Data[1].Id);
    }

    [Fact]
    public async Task List_WithFilter_IncludesFilterParametersInUrl()
    {
        SetupListResponse<Subscription>("[{\"id\":\"sub_1\"}]");
        var filter = new SubscriptionListFilter { CustomerId = "cus_1", IncludeDeleted = true };

        var result = await Manager.List(0, 10, filter);

        AssertRequestUrlContains("customer=cus_1");
        AssertRequestUrlContains("includeDeleted=True");
    }

    #endregion

    #region Update

    [Fact]
    public async Task Update_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"sub_123\",\"value\":129.90}");
        var request = new UpdateSubscriptionRequest { Value = 129.90m };

        var result = await Manager.Update("sub_123", request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/subscriptions/sub_123");
    }

    [Fact]
    public async Task Update_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"sub_123\",\"value\":129.90,\"description\":\"Updated plan\"}");
        var request = new UpdateSubscriptionRequest { Value = 129.90m, Description = "Updated plan" };

        var result = await Manager.Update("sub_123", request);

        Assert.True(result.WasSucessfull());
        Assert.Equal("sub_123", result.Data.Id);
        Assert.Equal(129.90m, result.Data.Value);
        Assert.Equal("Updated plan", result.Data.Description);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_SendsDeleteToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"sub_123\",\"deleted\":true}");

        var result = await Manager.Delete("sub_123");

        AssertRequestMethod(HttpMethod.Delete);
        AssertRequestUrl("/v3/subscriptions/sub_123");
    }

    [Fact]
    public async Task Delete_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"sub_123\",\"deleted\":true}");

        var result = await Manager.Delete("sub_123");

        Assert.True(result.WasSucessfull());
        Assert.Equal("sub_123", result.Data.Id);
        Assert.True(result.Data.Deleted);
    }

    #endregion

    #region ListPayments

    [Fact]
    public async Task ListPayments_SendsGetToCorrectUrl()
    {
        SetupListResponse<Payment>("[{\"id\":\"pay_1\",\"value\":99.90}]");

        var result = await Manager.ListPayments("sub_123", 0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/subscriptions/sub_123/payments");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    [Fact]
    public async Task ListPayments_DeserializesResponseCorrectly()
    {
        SetupListResponse<Payment>("[{\"id\":\"pay_1\",\"value\":99.90},{\"id\":\"pay_2\",\"value\":99.90}]", totalCount: 2);

        var result = await Manager.ListPayments("sub_123", 0, 10);

        Assert.True(result.WasSucessfull());
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("pay_1", result.Data[0].Id);
    }

    #endregion

    #region ListPaymentBook

    [Fact]
    public async Task ListPaymentBook_SendsGetToCorrectUrl()
    {
        SetupListResponse<Payment>("[{\"id\":\"pay_1\",\"value\":99.90}]");

        var result = await Manager.ListPaymentBook("sub_123", 0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/subscriptions/sub_123/paymentBook");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    [Fact]
    public async Task ListPaymentBook_DeserializesResponseCorrectly()
    {
        SetupListResponse<Payment>("[{\"id\":\"pay_1\",\"value\":99.90},{\"id\":\"pay_2\",\"value\":99.90},{\"id\":\"pay_3\",\"value\":99.90}]", totalCount: 3);

        var result = await Manager.ListPaymentBook("sub_123", 0, 10);

        Assert.True(result.WasSucessfull());
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Data.Count);
    }

    #endregion

    #region ListInvoice

    [Fact]
    public async Task ListInvoice_SendsGetToCorrectUrl()
    {
        SetupListResponse<Invoice>("[{\"id\":\"inv_1\",\"value\":99.90}]");

        var result = await Manager.ListInvoice("sub_123", 0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/subscriptions/sub_123/invoices");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    [Fact]
    public async Task ListInvoice_DeserializesResponseCorrectly()
    {
        SetupListResponse<Invoice>("[{\"id\":\"inv_1\",\"value\":99.90,\"status\":\"AUTHORIZED\"}]", totalCount: 1);

        var result = await Manager.ListInvoice("sub_123", 0, 10);

        Assert.True(result.WasSucessfull());
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Data);
        Assert.Equal("inv_1", result.Data[0].Id);
    }

    [Fact]
    public async Task ListInvoice_WithFilter_IncludesFilterParametersInUrl()
    {
        SetupListResponse<Invoice>("[{\"id\":\"inv_1\"}]");
        var filter = new SubscriptionInvoiceListFilter { InvoiceStatus = Models.Invoice.Enums.InvoiceStatus.AUTHORIZED };

        var result = await Manager.ListInvoice("sub_123", 0, 10, filter);

        AssertRequestUrlContains("status=AUTHORIZED");
    }

    #endregion

    #region CreateInvoiceSettings

    [Fact]
    public async Task CreateInvoiceSettings_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"municipalServiceId\":\"svc_1\",\"municipalServiceCode\":\"1234\",\"municipalServiceName\":\"Service\"}");
        var request = new CreateInvoiceSettingsRequest
        {
            MunicipalServiceId = "svc_1",
            MunicipalServiceCode = "1234",
            MunicipalServiceName = "Service"
        };

        var result = await Manager.CreateInvoiceSettings("sub_123", request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/subscriptions/sub_123/invoiceSettings");
    }

    [Fact]
    public async Task CreateInvoiceSettings_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"municipalServiceId\":\"svc_1\",\"municipalServiceCode\":\"1234\",\"municipalServiceName\":\"Service\",\"daysBeforeDueDate\":5,\"receivedOnly\":false}");
        var request = new CreateInvoiceSettingsRequest
        {
            MunicipalServiceId = "svc_1",
            MunicipalServiceCode = "1234",
            MunicipalServiceName = "Service"
        };

        var result = await Manager.CreateInvoiceSettings("sub_123", request);

        Assert.True(result.WasSucessfull());
        Assert.Equal("svc_1", result.Data.MunicipalServiceId);
        Assert.Equal("1234", result.Data.MunicipalServiceCode);
        Assert.Equal("Service", result.Data.MunicipalServiceName);
        Assert.Equal(5, result.Data.DaysBeforeDueDate);
        Assert.False(result.Data.ReceivedOnly);
    }

    #endregion

    #region UpdateInvoiceSettings

    [Fact]
    public async Task UpdateInvoiceSettings_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"municipalServiceId\":\"svc_1\",\"daysBeforeDueDate\":10,\"receivedOnly\":true}");
        var request = new UpdateInvoiceSettingsRequest { DaysBeforeDueDate = 10, ReceivedOnly = true };

        var result = await Manager.UpdateInvoiceSettings("sub_123", request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/subscriptions/sub_123/invoiceSettings");
    }

    [Fact]
    public async Task UpdateInvoiceSettings_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"municipalServiceId\":\"svc_1\",\"daysBeforeDueDate\":10,\"receivedOnly\":true}");
        var request = new UpdateInvoiceSettingsRequest { DaysBeforeDueDate = 10, ReceivedOnly = true };

        var result = await Manager.UpdateInvoiceSettings("sub_123", request);

        Assert.True(result.WasSucessfull());
        Assert.Equal(10, result.Data.DaysBeforeDueDate);
        Assert.True(result.Data.ReceivedOnly);
    }

    #endregion

    #region FindInvoiceSettings

    [Fact]
    public async Task FindInvoiceSettings_SendsGetToCorrectUrl()
    {
        SetupOkResponse("{\"municipalServiceId\":\"svc_1\",\"municipalServiceCode\":\"1234\",\"municipalServiceName\":\"Service\"}");

        var result = await Manager.FindInvoiceSettings("sub_123");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/subscriptions/sub_123/invoiceSettings");
    }

    [Fact]
    public async Task FindInvoiceSettings_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"municipalServiceId\":\"svc_1\",\"municipalServiceCode\":\"1234\",\"municipalServiceName\":\"Service\",\"daysBeforeDueDate\":5}");

        var result = await Manager.FindInvoiceSettings("sub_123");

        Assert.True(result.WasSucessfull());
        Assert.Equal("svc_1", result.Data.MunicipalServiceId);
        Assert.Equal(5, result.Data.DaysBeforeDueDate);
    }

    #endregion

    #region DeleteInvoiceSettings

    [Fact]
    public async Task DeleteInvoiceSettings_SendsDeleteToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"sub_123\",\"deleted\":true}");

        var result = await Manager.DeleteInvoiceSettings("sub_123");

        AssertRequestMethod(HttpMethod.Delete);
        AssertRequestUrl("/v3/subscriptions/sub_123/invoiceSettings");
    }

    [Fact]
    public async Task DeleteInvoiceSettings_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"sub_123\",\"deleted\":true}");

        var result = await Manager.DeleteInvoiceSettings("sub_123");

        Assert.True(result.WasSucessfull());
        Assert.Equal("sub_123", result.Data.Id);
        Assert.True(result.Data.Deleted);
    }

    #endregion
}
