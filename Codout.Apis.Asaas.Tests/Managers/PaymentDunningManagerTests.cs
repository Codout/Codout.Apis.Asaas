using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.Common;
using Codout.Apis.Asaas.Models.PaymentDunning;
using Codout.Apis.Asaas.Models.PaymentDunning.Enums;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class PaymentDunningManagerTests : ManagerTestBase<PaymentDunningManager>
{
    protected override PaymentDunningManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestablePaymentDunningManager(settings, handler);

    // ── Create ──────────────────────────────────────────────────────

    [Fact]
    public async Task Create_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"dun_123\",\"status\":\"PENDING\"}");

        var request = new CreatePaymentDunningRequest
        {
            PaymentId = "pay_1",
            Type = PaymentDunningType.CREDIT_BUREAU,
            Description = "Dunning for overdue payment",
            CustomerName = "Test Customer",
            CustomerCpfCnpj = "12345678901",
            CustomerPrimaryPhone = "11999998888",
            CustomerSecondaryPhone = "11888887777",
            CustomerPostalCode = "01001000",
            CustomerAddress = "Rua Principal",
            CustomerAddressNumber = "100",
            CustomerComplement = "Sala 1",
            CustomerProvince = "Centro",
            Documents = new List<AsaasFile>()
        };

        var result = await Manager.Create(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/paymentDunnings");
    }

    [Fact]
    public async Task Create_DeserializesResponse()
    {
        SetupOkResponse("{\"id\":\"dun_123\",\"dunningNumber\":\"DUN001\",\"status\":\"PENDING\",\"type\":\"CREDIT_BUREAU\",\"payment\":\"pay_1\",\"requestDate\":\"2024-01-10\",\"description\":\"Dunning for overdue payment\",\"value\":500.00,\"feeValue\":25.00,\"netValue\":475.00,\"receivedInCashFeeValue\":10.00,\"canBeCancelled\":true,\"isNecessaryResendDocumentation\":false}");

        var request = new CreatePaymentDunningRequest
        {
            PaymentId = "pay_1",
            Type = PaymentDunningType.CREDIT_BUREAU,
            Description = "Dunning for overdue payment",
            CustomerName = "Test Customer",
            CustomerCpfCnpj = "12345678901",
            CustomerPrimaryPhone = "11999998888",
            CustomerSecondaryPhone = "11888887777",
            CustomerPostalCode = "01001000",
            CustomerAddress = "Rua Principal",
            CustomerAddressNumber = "100",
            CustomerComplement = "Sala 1",
            CustomerProvince = "Centro",
            Documents = new List<AsaasFile>()
        };

        var result = await Manager.Create(request);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("dun_123", result.Data.Id);
        Assert.Equal("DUN001", result.Data.DunningNumber);
        Assert.Equal("pay_1", result.Data.PaymentId);
        Assert.Equal("Dunning for overdue payment", result.Data.Description);
        Assert.Equal(500.00m, result.Data.Value);
        Assert.Equal(25.00m, result.Data.FeeValue);
        Assert.Equal(475.00m, result.Data.NetValue);
        Assert.Equal(10.00m, result.Data.ReceivedInCashFeeValue);
        Assert.True(result.Data.CanBeCancelled);
        Assert.False(result.Data.IsNecessaryResendDocumentation);
    }

    // ── Simulate ────────────────────────────────────────────────────

    [Fact]
    public async Task Simulate_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"payment\":\"pay_1\",\"value\":500.00}");

        var request = new SimulatePaymentDunningRequest { PaymentId = "pay_1" };

        var result = await Manager.Simulate(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/paymentDunnings/simulate");
    }

    [Fact]
    public async Task Simulate_DeserializesResponse()
    {
        SetupOkResponse("{\"payment\":\"pay_1\",\"value\":500.00,\"typeSimulations\":null}");

        var request = new SimulatePaymentDunningRequest { PaymentId = "pay_1" };

        var result = await Manager.Simulate(request);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("pay_1", result.Data.PaymentId);
        Assert.Equal(500.00m, result.Data.Value);
    }

    // ── Find ────────────────────────────────────────────────────────

    [Fact]
    public async Task Find_SendsGetToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"dun_456\",\"status\":\"PROCESSED\"}");

        var result = await Manager.Find("dun_456");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/paymentDunnings/dun_456");
    }

    [Fact]
    public async Task Find_DeserializesResponse()
    {
        SetupOkResponse("{\"id\":\"dun_456\",\"status\":\"PROCESSED\",\"value\":300.00,\"feeValue\":15.00,\"netValue\":285.00}");

        var result = await Manager.Find("dun_456");

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("dun_456", result.Data.Id);
        Assert.Equal(300.00m, result.Data.Value);
        Assert.Equal(15.00m, result.Data.FeeValue);
        Assert.Equal(285.00m, result.Data.NetValue);
    }

    // ── List ────────────────────────────────────────────────────────

    [Fact]
    public async Task List_SendsGetToCorrectUrl()
    {
        SetupListResponse<PaymentDunning>("[]", totalCount: 0);

        var result = await Manager.List(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/paymentDunnings");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    [Fact]
    public async Task List_WithFilter_IncludesFilterParameters()
    {
        SetupListResponse<PaymentDunning>("[]", totalCount: 0);

        var filter = new PaymentDunningListFilter
        {
            PaymentId = "pay_999"
        };

        var result = await Manager.List(0, 10, filter);

        AssertRequestUrlContains("payment=pay_999");
    }

    [Fact]
    public async Task List_DeserializesResponse()
    {
        SetupListResponse<PaymentDunning>("[{\"id\":\"dun_1\",\"value\":100.00},{\"id\":\"dun_2\",\"value\":200.00}]", totalCount: 2);

        var result = await Manager.List(0, 10);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal("dun_1", result.Data[0].Id);
        Assert.Equal("dun_2", result.Data[1].Id);
    }

    [Fact]
    public async Task List_WithoutFilter_DoesNotThrow()
    {
        SetupListResponse<PaymentDunning>("[]", totalCount: 0);

        var result = await Manager.List(0, 10, null);

        Assert.True(result.WasSucessfull());
    }

    // ── ListEventHistory ────────────────────────────────────────────

    [Fact]
    public async Task ListEventHistory_SendsGetToCorrectUrl()
    {
        SetupListResponse<PaymentDunningEventHistory>("[]", totalCount: 0);

        var result = await Manager.ListEventHistory("dun_123", 0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/paymentDunnings/dun_123/history");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    [Fact]
    public async Task ListEventHistory_DeserializesResponse()
    {
        SetupListResponse<PaymentDunningEventHistory>("[{\"status\":\"CREATED\",\"description\":\"Dunning created\",\"eventDate\":\"2024-01-10\"},{\"status\":\"SENT\",\"description\":\"Dunning sent\",\"eventDate\":\"2024-01-11\"}]", totalCount: 2);

        var result = await Manager.ListEventHistory("dun_123", 0, 10);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("CREATED", result.Data[0].Status);
        Assert.Equal("Dunning created", result.Data[0].Description);
    }

    // ── ListPartialPaymentsReceived ─────────────────────────────────

    [Fact]
    public async Task ListPartialPaymentsReceived_SendsGetToCorrectUrl()
    {
        SetupListResponse<PaymentDunningPartialPayments>("[]", totalCount: 0);

        var result = await Manager.ListPartialPaymentsReceived("dun_123", 0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/paymentDunnings/dun_123/partialPayments");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    [Fact]
    public async Task ListPartialPaymentsReceived_DeserializesResponse()
    {
        SetupListResponse<PaymentDunningPartialPayments>("[{\"value\":150.00,\"description\":\"Partial payment\",\"paymentDate\":\"2024-02-01\"}]", totalCount: 1);

        var result = await Manager.ListPartialPaymentsReceived("dun_123", 0, 10);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        Assert.Equal(150.00m, result.Data[0].Value);
        Assert.Equal("Partial payment", result.Data[0].Description);
    }

    // ── ListPaymentsAvailableForDunning ─────────────────────────────

    [Fact]
    public async Task ListPaymentsAvailableForDunning_SendsGetToCorrectUrl()
    {
        SetupListResponse<PaymentDunningPaymentAvailable>("[]", totalCount: 0);

        var result = await Manager.ListPaymentsAvailableForDunning(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/paymentDunnings/paymentsAvailableForDunning");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    [Fact]
    public async Task ListPaymentsAvailableForDunning_DeserializesResponse()
    {
        SetupListResponse<PaymentDunningPaymentAvailable>("[{\"payment\":\"pay_1\",\"customer\":\"cust_1\",\"value\":1000.00,\"dueDate\":\"2024-01-15\"}]", totalCount: 1);

        var result = await Manager.ListPaymentsAvailableForDunning(0, 10);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        Assert.Equal("pay_1", result.Data[0].PaymentId);
        Assert.Equal("cust_1", result.Data[0].CustomerId);
        Assert.Equal(1000.00m, result.Data[0].Value);
    }

    // ── ResendDocument ──────────────────────────────────────────────

    [Fact]
    public async Task ResendDocument_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"dun_123\",\"status\":\"PENDING\"}");

        var result = await Manager.ResendDocument("dun_123", []);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/paymentDunnings/dun_123/documents");
    }

    // ── Cancel ──────────────────────────────────────────────────────

    [Fact]
    public async Task Cancel_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"dun_123\",\"status\":\"CANCELLED\"}");

        var result = await Manager.Cancel("dun_123");

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/paymentDunnings/dun_123/cancel");
    }

    [Fact]
    public async Task Cancel_DeserializesResponse()
    {
        SetupOkResponse("{\"id\":\"dun_123\",\"status\":\"CANCELLED\",\"value\":500.00,\"cancellationFeeValue\":10.00}");

        var result = await Manager.Cancel("dun_123");

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("dun_123", result.Data.Id);
        Assert.Equal(10.00m, result.Data.CancellationFeeValue);
    }

    // ── Error handling ──────────────────────────────────────────────

    [Fact]
    public async Task Find_OnError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.NotFound);

        var result = await Manager.Find("dun_nonexistent");

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("invalid", result.Errors[0].Code);
    }

    [Fact]
    public async Task Cancel_OnError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.BadRequest);

        var result = await Manager.Cancel("dun_invalid");

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotEmpty(result.Errors);
    }
}
