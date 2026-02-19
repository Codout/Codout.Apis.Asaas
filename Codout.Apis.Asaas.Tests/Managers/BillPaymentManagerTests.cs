using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.Bill;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class BillPaymentManagerTests : ManagerTestBase<BillPaymentManager>
{
    protected override BillPaymentManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableBillPaymentManager(settings, handler);

    // ── Create ──────────────────────────────────────────────────────

    [Fact]
    public async Task Create_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"bill_123\",\"status\":\"PENDING\"}");

        var request = new CreateBillPaymentRequest
        {
            IdentificationField = "23793.38128 60000.000003 00000.000406 1 84340000010000",
            Description = "Electric bill",
            Value = 100.00m
        };

        var result = await Manager.Create(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/bill");
    }

    [Fact]
    public async Task Create_DeserializesResponse()
    {
        SetupOkResponse("{\"id\":\"bill_123\",\"status\":\"PENDING\",\"value\":100.00,\"discount\":0.00,\"identificationField\":\"23793.38128 60000.000003 00000.000406 1 84340000010000\",\"dueDate\":\"2024-02-15\",\"scheduleDate\":\"2024-02-14\",\"fee\":1.50,\"description\":\"Electric bill\",\"companyName\":\"Energy Co\",\"transactionReceiptUrl\":\"https://example.com/receipt\",\"canBeCancelled\":true,\"failReasons\":null}");

        var request = new CreateBillPaymentRequest
        {
            IdentificationField = "23793.38128",
            Description = "Electric bill",
            Value = 100.00m
        };

        var result = await Manager.Create(request);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("bill_123", result.Data.Id);
        Assert.Equal(100.00m, result.Data.Value);
        Assert.Equal(0.00m, result.Data.Discount);
        Assert.Equal(1.50m, result.Data.Fee);
        Assert.Equal("Electric bill", result.Data.Description);
        Assert.Equal("Energy Co", result.Data.CompanyName);
        Assert.Equal("https://example.com/receipt", result.Data.TransactionReceiptUrl);
        Assert.True(result.Data.CanBeCancelled);
        Assert.Null(result.Data.FailReasons);
    }

    // ── Simulate ────────────────────────────────────────────────────

    [Fact]
    public async Task Simulate_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"minimumScheduleDate\":\"2024-02-14\",\"fee\":1.50}");

        var request = new SimulateBillPaymentRequest
        {
            IdentificationField = "23793.38128"
        };

        var result = await Manager.Simulate(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/bill/simulate");
    }

    [Fact]
    public async Task Simulate_DeserializesResponse()
    {
        SetupOkResponse("{\"minimumScheduleDate\":\"2024-02-14\",\"fee\":2.50,\"bankSlipInfo\":{\"identificationField\":\"23793.38128\",\"dueDate\":\"2024-02-15\",\"value\":100.00,\"companyName\":\"Energy Co\"}}");

        var request = new SimulateBillPaymentRequest { IdentificationField = "23793.38128" };

        var result = await Manager.Simulate(request);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal(2.50m, result.Data.Fee);
        Assert.NotNull(result.Data.BankSlipInfo);
    }

    // ── Find ────────────────────────────────────────────────────────

    [Fact]
    public async Task Find_SendsGetToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"bill_456\",\"status\":\"PAID\"}");

        var result = await Manager.Find("bill_456");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/bill/bill_456");
    }

    [Fact]
    public async Task Find_DeserializesResponse()
    {
        SetupOkResponse("{\"id\":\"bill_456\",\"status\":\"PAID\",\"value\":250.00,\"fee\":2.00,\"description\":\"Phone bill\",\"companyName\":\"Telecom Co\",\"canBeCancelled\":false}");

        var result = await Manager.Find("bill_456");

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("bill_456", result.Data.Id);
        Assert.Equal(250.00m, result.Data.Value);
        Assert.Equal(2.00m, result.Data.Fee);
        Assert.Equal("Phone bill", result.Data.Description);
        Assert.Equal("Telecom Co", result.Data.CompanyName);
        Assert.False(result.Data.CanBeCancelled);
    }

    // ── List ────────────────────────────────────────────────────────

    [Fact]
    public async Task List_SendsGetToCorrectUrl()
    {
        SetupListResponse<BillPayment>("[]", totalCount: 0);

        var result = await Manager.List(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/bill");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    [Fact]
    public async Task List_WithPagination_IncludesQueryParameters()
    {
        SetupListResponse<BillPayment>("[]", totalCount: 0);

        var result = await Manager.List(20, 50);

        AssertRequestUrlContains("offset=20");
        AssertRequestUrlContains("limit=50");
    }

    [Fact]
    public async Task List_DeserializesResponse()
    {
        SetupListResponse<BillPayment>("[{\"id\":\"bill_1\",\"value\":100.00},{\"id\":\"bill_2\",\"value\":200.00},{\"id\":\"bill_3\",\"value\":300.00}]", totalCount: 3, hasMore: false);

        var result = await Manager.List(0, 10);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.False(result.HasMore);
        Assert.Equal("bill_1", result.Data[0].Id);
        Assert.Equal(100.00m, result.Data[0].Value);
        Assert.Equal("bill_2", result.Data[1].Id);
        Assert.Equal("bill_3", result.Data[2].Id);
    }

    // ── Cancel ──────────────────────────────────────────────────────

    [Fact]
    public async Task Cancel_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"bill_123\",\"status\":\"CANCELLED\"}");

        var result = await Manager.Cancel("bill_123");

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/bill/bill_123/cancel");
    }

    [Fact]
    public async Task Cancel_DeserializesResponse()
    {
        SetupOkResponse("{\"id\":\"bill_123\",\"status\":\"CANCELLED\",\"value\":100.00,\"canBeCancelled\":false}");

        var result = await Manager.Cancel("bill_123");

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("bill_123", result.Data.Id);
        Assert.False(result.Data.CanBeCancelled);
    }

    // ── Error handling ──────────────────────────────────────────────

    [Fact]
    public async Task Create_OnError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.BadRequest);

        var request = new CreateBillPaymentRequest { IdentificationField = "invalid" };

        var result = await Manager.Create(request);

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("invalid", result.Errors[0].Code);
        Assert.Equal("Test error", result.Errors[0].Description);
    }

    [Fact]
    public async Task Find_OnError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.NotFound);

        var result = await Manager.Find("bill_nonexistent");

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task Cancel_OnError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.UnprocessableEntity);

        var result = await Manager.Cancel("bill_already_paid");

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
        Assert.NotEmpty(result.Errors);
    }
}
