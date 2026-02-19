using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using System.Collections.Generic;
using Codout.Apis.Asaas.Models.Anticipation;
using Codout.Apis.Asaas.Models.Common;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class AnticipationManagerTests : ManagerTestBase<AnticipationManager>
{
    protected override AnticipationManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableAnticipationManager(settings, handler);

    // ── Create ──────────────────────────────────────────────────────

    [Fact]
    public async Task Create_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"ant_123\",\"status\":\"PENDING\",\"totalValue\":100.0}");

        var request = new CreateAnticipationRequest
        {
            InstallmentId = "inst_1",
            PaymentId = "pay_1",
            AgreementSignature = "sig_1",
            Documents = new List<AsaasFile>()
        };

        var result = await Manager.Create(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/anticipations");
    }

    [Fact]
    public async Task Create_DeserializesResponse()
    {
        SetupOkResponse("{\"id\":\"ant_123\",\"installment\":\"inst_1\",\"payment\":\"pay_1\",\"status\":\"PENDING\",\"anticipationDate\":\"2024-01-15\",\"dueDate\":\"2024-02-15\",\"requestDate\":\"2024-01-10\",\"anticipationDays\":30,\"totalValue\":100.50,\"fee\":5.25,\"netValue\":95.25,\"value\":100.50}");

        var request = new CreateAnticipationRequest
        {
            InstallmentId = "inst_1",
            PaymentId = "pay_1",
            AgreementSignature = "sig_1",
            Documents = new List<AsaasFile>()
        };

        var result = await Manager.Create(request);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("ant_123", result.Data.Id);
        Assert.Equal("inst_1", result.Data.InstallmentId);
        Assert.Equal("pay_1", result.Data.PaymentId);
        Assert.Equal(30, result.Data.AnticipationDays);
        Assert.Equal(100.50m, result.Data.TotalValue);
        Assert.Equal(5.25m, result.Data.Fee);
        Assert.Equal(95.25m, result.Data.NetValue);
    }

    // ── Simulate ────────────────────────────────────────────────────

    [Fact]
    public async Task Simulate_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"payment\":\"pay_1\",\"totalValue\":100.0,\"fee\":5.0,\"netValue\":95.0}");

        var request = new SimulateAnticipationRequest { PaymentId = "pay_1" };

        var result = await Manager.Simulate(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/anticipations/simulate");
    }

    [Fact]
    public async Task Simulate_DeserializesResponse()
    {
        SetupOkResponse("{\"installment\":\"inst_1\",\"payment\":\"pay_1\",\"anticipationDate\":\"2024-01-15\",\"dueDate\":\"2024-02-15\",\"anticipationDays\":30,\"totalValue\":200.00,\"fee\":10.00,\"netValue\":190.00,\"value\":200.00,\"isDocumentationRequired\":true}");

        var request = new SimulateAnticipationRequest { PaymentId = "pay_1" };

        var result = await Manager.Simulate(request);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("inst_1", result.Data.InstallmentId);
        Assert.Equal("pay_1", result.Data.PaymentId);
        Assert.Equal(200.00m, result.Data.TotalValue);
        Assert.Equal(10.00m, result.Data.Fee);
        Assert.Equal(190.00m, result.Data.NetValue);
        Assert.True(result.Data.IsDocumentationRequired);
    }

    // ── Find ────────────────────────────────────────────────────────

    [Fact]
    public async Task Find_SendsGetToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"ant_456\",\"status\":\"CREDITED\"}");

        var result = await Manager.Find("ant_456");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/anticipations/ant_456");
    }

    [Fact]
    public async Task Find_DeserializesResponse()
    {
        SetupOkResponse("{\"id\":\"ant_456\",\"status\":\"CREDITED\",\"totalValue\":500.00,\"fee\":25.00,\"netValue\":475.00,\"denialObservation\":null}");

        var result = await Manager.Find("ant_456");

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("ant_456", result.Data.Id);
        Assert.Equal(500.00m, result.Data.TotalValue);
        Assert.Equal(25.00m, result.Data.Fee);
        Assert.Equal(475.00m, result.Data.NetValue);
    }

    // ── List ────────────────────────────────────────────────────────

    [Fact]
    public async Task List_SendsGetToCorrectUrl()
    {
        SetupListResponse<Anticipation>("[]", totalCount: 0);

        var result = await Manager.List(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/anticipations");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    [Fact]
    public async Task List_WithFilter_IncludesFilterParameters()
    {
        SetupListResponse<Anticipation>("[]", totalCount: 0);

        var filter = new AnticipationListFilter
        {
            PaymentId = "pay_123"
        };

        var result = await Manager.List(0, 10, filter);

        AssertRequestUrlContains("payment=pay_123");
    }

    [Fact]
    public async Task List_DeserializesResponse()
    {
        SetupListResponse<Anticipation>("[{\"id\":\"ant_1\",\"totalValue\":100.0},{\"id\":\"ant_2\",\"totalValue\":200.0}]", totalCount: 2);

        var result = await Manager.List(0, 10);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal("ant_1", result.Data[0].Id);
        Assert.Equal("ant_2", result.Data[1].Id);
    }

    // ── SignAgreement ───────────────────────────────────────────────

    [Fact]
    public async Task SignAgreement_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"ant_789\",\"status\":\"CREDITED\"}");

        var request = new SignAnticipationAgreementRequest { Agreed = true };

        var result = await Manager.SignAgreement(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/anticipations/agreement/sign");
    }

    [Fact]
    public async Task SignAgreement_DeserializesResponse()
    {
        SetupOkResponse("{\"id\":\"ant_789\",\"status\":\"CREDITED\",\"totalValue\":300.00}");

        var request = new SignAnticipationAgreementRequest { Agreed = true };

        var result = await Manager.SignAgreement(request);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("ant_789", result.Data.Id);
    }

    // ── Error handling ──────────────────────────────────────────────

    [Fact]
    public async Task Find_OnError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.NotFound);

        var result = await Manager.Find("ant_nonexistent");

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task Create_OnError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.BadRequest);

        var request = new CreateAnticipationRequest
        {
            InstallmentId = "inst_1",
            PaymentId = "pay_1",
            AgreementSignature = "sig_1",
            Documents = new List<AsaasFile>()
        };
        var result = await Manager.Create(request);

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("invalid", result.Errors[0].Code);
    }
}
