using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.Invoice;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class InvoiceManagerTests : ManagerTestBase<InvoiceManager>
{
    protected override InvoiceManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableInvoiceManager(settings, handler);

    // ── Schedule ────────────────────────────────────────────────────

    [Fact]
    public async Task Schedule_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"inv_123\",\"status\":\"SCHEDULED\"}");

        var request = new CreateInvoiceRequest
        {
            PaymentId = "pay_1",
            ServiceDescription = "Consulting",
            Value = 1000.00m
        };

        var result = await Manager.Schedule(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/invoices");
    }

    [Fact]
    public async Task Schedule_DeserializesResponse()
    {
        SetupOkResponse("{\"id\":\"inv_123\",\"status\":\"SCHEDULED\",\"customer\":\"cust_1\",\"payment\":\"pay_1\",\"installment\":null,\"type\":\"NFS-e\",\"statusDescription\":\"Agendada\",\"serviceDescription\":\"Consulting\",\"pdfUrl\":\"https://example.com/inv.pdf\",\"xmlUrl\":\"https://example.com/inv.xml\",\"rpsSerie\":\"RPS\",\"rpsNumber\":\"001\",\"number\":\"12345\",\"validationCode\":\"ABC123\",\"value\":1000.00,\"deductions\":0.00,\"observations\":\"Test\",\"municipalServiceId\":\"ms_1\",\"municipalServiceCode\":\"01.01\",\"municipalServiceName\":\"IT Service\"}");

        var request = new CreateInvoiceRequest { PaymentId = "pay_1", Value = 1000.00m };

        var result = await Manager.Schedule(request);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("inv_123", result.Data.Id);
        Assert.Equal("cust_1", result.Data.CustomerId);
        Assert.Equal("pay_1", result.Data.PaymentId);
        Assert.Equal("NFS-e", result.Data.Type);
        Assert.Equal("Consulting", result.Data.ServiceDescription);
        Assert.Equal("https://example.com/inv.pdf", result.Data.PdfUrl);
        Assert.Equal("https://example.com/inv.xml", result.Data.XmlUrl);
        Assert.Equal("RPS", result.Data.RpsSerie);
        Assert.Equal("001", result.Data.RpsNumber);
        Assert.Equal("12345", result.Data.Number);
        Assert.Equal("ABC123", result.Data.ValidationCode);
        Assert.Equal(1000.00m, result.Data.Value);
        Assert.Equal(0.00m, result.Data.Deductions);
        Assert.Equal("01.01", result.Data.MunicipalServiceCode);
    }

    // ── Update ──────────────────────────────────────────────────────

    [Fact]
    public async Task Update_SendsPutToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"inv_123\",\"status\":\"SCHEDULED\"}");

        var request = new UpdateInvoiceRequest
        {
            ServiceDescription = "Updated consulting",
            Value = 1500.00m
        };

        var result = await Manager.Update("inv_123", request);

        AssertRequestMethod(HttpMethod.Put);
        AssertRequestUrl("/v3/invoices/inv_123");
    }

    [Fact]
    public async Task Update_DeserializesResponse()
    {
        SetupOkResponse("{\"id\":\"inv_123\",\"status\":\"SCHEDULED\",\"serviceDescription\":\"Updated consulting\",\"value\":1500.00}");

        var request = new UpdateInvoiceRequest { ServiceDescription = "Updated consulting", Value = 1500.00m };

        var result = await Manager.Update("inv_123", request);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("inv_123", result.Data.Id);
        Assert.Equal("Updated consulting", result.Data.ServiceDescription);
        Assert.Equal(1500.00m, result.Data.Value);
    }

    // ── Find ────────────────────────────────────────────────────────

    [Fact]
    public async Task Find_SendsGetToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"inv_456\",\"status\":\"AUTHORIZED\"}");

        var result = await Manager.Find("inv_456");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/invoices/inv_456");
    }

    [Fact]
    public async Task Find_DeserializesResponse()
    {
        SetupOkResponse("{\"id\":\"inv_456\",\"status\":\"AUTHORIZED\",\"value\":2000.00,\"serviceDescription\":\"Development\"}");

        var result = await Manager.Find("inv_456");

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("inv_456", result.Data.Id);
        Assert.Equal(2000.00m, result.Data.Value);
        Assert.Equal("Development", result.Data.ServiceDescription);
    }

    // ── List ────────────────────────────────────────────────────────

    [Fact]
    public async Task List_SendsGetToCorrectUrl()
    {
        SetupListResponse<Invoice>("[]", totalCount: 0);

        var result = await Manager.List(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/invoices");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    [Fact]
    public async Task List_WithFilter_IncludesFilterParameters()
    {
        SetupListResponse<Invoice>("[]", totalCount: 0);

        var filter = new InvoiceListFilter
        {
            PaymentId = "pay_123"
        };

        var result = await Manager.List(0, 10, filter);

        AssertRequestUrlContains("payment=pay_123");
    }

    [Fact]
    public async Task List_DeserializesResponse()
    {
        SetupListResponse<Invoice>("[{\"id\":\"inv_1\",\"value\":100.0},{\"id\":\"inv_2\",\"value\":200.0}]", totalCount: 2, hasMore: true);

        var result = await Manager.List(0, 10);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal(2, result.TotalCount);
        Assert.True(result.HasMore);
        Assert.Equal("inv_1", result.Data[0].Id);
        Assert.Equal("inv_2", result.Data[1].Id);
    }

    [Fact]
    public async Task List_WithoutFilter_DoesNotIncludeFilterParameters()
    {
        SetupListResponse<Invoice>("[]", totalCount: 0);

        var result = await Manager.List(0, 10);

        AssertRequestUrlContains("/v3/invoices");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    // ── Authorize ───────────────────────────────────────────────────

    [Fact]
    public async Task Authorize_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"inv_123\",\"status\":\"AUTHORIZED\"}");

        var result = await Manager.Authorize("inv_123");

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/invoices/inv_123/authorize");
    }

    [Fact]
    public async Task Authorize_DeserializesResponse()
    {
        SetupOkResponse("{\"id\":\"inv_123\",\"status\":\"AUTHORIZED\",\"value\":500.00}");

        var result = await Manager.Authorize("inv_123");

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("inv_123", result.Data.Id);
    }

    // ── Cancel ──────────────────────────────────────────────────────

    [Fact]
    public async Task Cancel_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"inv_123\",\"status\":\"CANCELED\"}");

        var result = await Manager.Cancel("inv_123");

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/invoices/inv_123/cancel");
    }

    [Fact]
    public async Task Cancel_DeserializesResponse()
    {
        SetupOkResponse("{\"id\":\"inv_123\",\"status\":\"CANCELED\",\"value\":500.00}");

        var result = await Manager.Cancel("inv_123");

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("inv_123", result.Data.Id);
    }

    // ── ListMunicipalServices ───────────────────────────────────────

    [Fact]
    public async Task ListMunicipalServices_SendsGetToCorrectUrl()
    {
        SetupListResponse<MunicipalService>("[]", totalCount: 0);

        var result = await Manager.ListMunicipalServices("IT");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/invoices/municipalServices");
        AssertRequestUrlContains("description=IT");
    }

    [Fact]
    public async Task ListMunicipalServices_DeserializesResponse()
    {
        SetupListResponse<MunicipalService>("[{\"id\":\"ms_1\",\"description\":\"IT Service\",\"iss\":5.0},{\"id\":\"ms_2\",\"description\":\"IT Consulting\",\"iss\":3.0}]", totalCount: 2);

        var result = await Manager.ListMunicipalServices("IT");

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("ms_1", result.Data[0].Id);
        Assert.Equal("IT Service", result.Data[0].Description);
        Assert.Equal(5.0m, result.Data[0].Iss);
        Assert.Equal("ms_2", result.Data[1].Id);
    }

    // ── Error handling ──────────────────────────────────────────────

    [Fact]
    public async Task Schedule_OnError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.BadRequest);

        var request = new CreateInvoiceRequest { PaymentId = "pay_invalid" };
        var result = await Manager.Schedule(request);

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("invalid", result.Errors[0].Code);
    }

    [Fact]
    public async Task Find_OnError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.NotFound);

        var result = await Manager.Find("inv_nonexistent");

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.NotEmpty(result.Errors);
    }
}
