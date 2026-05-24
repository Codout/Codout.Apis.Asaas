using System;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.PaymentDunning;
using Codout.Apis.Asaas.Models.PaymentDunning.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para PaymentDunningManager.
/// Schemas verificados via MCP em 2026-05-24:
/// - POST /v3/paymentDunnings (multipart, SaveRequest com 9 campos obrigatorios)
/// - GET /v3/paymentDunnings (envelope padrao, filtros status/type/payment/datas)
/// - POST /v3/paymentDunnings/simulate (payment como QUERY param, body vazio!)
/// - GET /v3/paymentDunnings/{id}
/// - GET /v3/paymentDunnings/{id}/history (status enum NEGOTIATED/PAID/etc)
/// - GET /v3/paymentDunnings/{id}/partialPayments
/// - GET /v3/paymentDunnings/paymentsAvailableForDunning
/// - POST /v3/paymentDunnings/{id}/cancel (body vazio)
/// </summary>
public class PaymentDunningContractTests
{
    // ─────────────────────────────────────────────────────────────
    // PaymentDunning response shape - B-22a/b/c/d regression coverage
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void DunningResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("PaymentDunning/dunning-response.json");

        var result = JsonContractAssert.DeserializeFixture<PaymentDunning>(json);

        Assert.Equal("ce35702d-0d9f-475a-ba46-e251ad265c91", result.Id);
        // B-22a: DunningNumber agora e int? (schema integer), antes era string.
        Assert.Equal(15, result.DunningNumber);
        Assert.Equal(PaymentDunningStatus.PENDING, result.Status);
        Assert.Equal(PaymentDunningType.CREDIT_BUREAU, result.Type);
        Assert.Equal(new DateTime(2020, 5, 26), result.RequestDate);
        Assert.Equal(80m, result.Value);
        Assert.Equal(8m, result.FeeValue);
        Assert.Equal(72m, result.NetValue);
        Assert.Equal("pay_080225913252", result.PaymentId);
        // B-22c/d: nullable bools
        Assert.True(result.CanBeCancelled);
        Assert.False(result.IsNecessaryResendDocumentation);
        // B-22b: CannotBeCancelledReason novo campo
        Assert.Null(result.CannotBeCancelledReason);
    }

    [Fact]
    public void DunningResponse_HandlesNullableBools()
    {
        // B-22c/d: canBeCancelled e isNecessaryResendDocumentation podem vir
        // omitidos no JSON. Antes do fix, eram bool nao-nulavel, o que
        // forcaria false silenciosamente.
        var json = "{\"id\":\"x\",\"status\":\"PENDING\"}";

        var result = JsonContractAssert.DeserializeFixture<PaymentDunning>(json);

        Assert.Null(result.CanBeCancelled);
        Assert.Null(result.IsNecessaryResendDocumentation);
    }

    [Fact]
    public void DunningsList_UsesStandardEnvelopeWithPagination()
    {
        var json = FixtureLoader.Load("PaymentDunning/dunnings-list-response.json");
        var response = new ResponseList<PaymentDunning>(System.Net.HttpStatusCode.OK, json);

        Assert.True(response.WasSuccessful());
        Assert.Equal(1, response.TotalCount);
        Assert.False(response.HasMore);
        Assert.Single(response.Data);
    }

    [Fact]
    public void DunningStatus_AllEightValuesDeserialize()
    {
        foreach (var status in new[] {
            "PENDING", "AWAITING_APPROVAL", "AWAITING_CANCELLATION", "PROCESSED",
            "PAID", "PARTIALLY_PAID", "DENIED", "CANCELLED" })
        {
            var json = $"{{\"id\":\"x\",\"status\":\"{status}\"}}";
            var result = JsonContractAssert.DeserializeFixture<PaymentDunning>(json);
            Assert.Equal(status, result.Status.ToString());
        }
    }

    [Fact]
    public void DunningType_BothValuesDeserialize()
    {
        // Save/response so retornam CREDIT_BUREAU. DEBT_RECOVERY_ASSISTANCE
        // existe apenas no filter, mas o enum precisa aceitar ambos para
        // que o filter compile.
        foreach (var type in new[] { "CREDIT_BUREAU", "DEBT_RECOVERY_ASSISTANCE" })
        {
            var json = $"{{\"id\":\"x\",\"status\":\"PENDING\",\"type\":\"{type}\"}}";
            var result = JsonContractAssert.DeserializeFixture<PaymentDunning>(json);
            Assert.Equal(type, result.Type.ToString());
        }
    }

    // ─────────────────────────────────────────────────────────────
    // PaymentDunningEventHistory - B-22f regression
    // Status era string, deve ser PaymentDunningHistoryStatus enum.
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void HistoryResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("PaymentDunning/history-list-response.json");
        var response = new ResponseList<PaymentDunningEventHistory>(System.Net.HttpStatusCode.OK, json);

        Assert.True(response.WasSuccessful());
        Assert.Single(response.Data);
        Assert.Equal(PaymentDunningHistoryStatus.NEGOTIATED, response.Data[0].Status);
        Assert.Equal(new DateTime(2019, 2, 20), response.Data[0].EventDate);
    }

    [Fact]
    public void HistoryStatus_AllFourValuesDeserialize()
    {
        // Schema: IN_NEGOTIATION, NEGOTIATION_FAIL, NEGOTIATED, PAID
        foreach (var status in new[] { "IN_NEGOTIATION", "NEGOTIATION_FAIL", "NEGOTIATED", "PAID" })
        {
            var json = $"{{\"status\":\"{status}\",\"eventDate\":\"2024-01-01\"}}";
            var result = JsonContractAssert.DeserializeFixture<PaymentDunningEventHistory>(json);
            Assert.Equal(status, result.Status.ToString());
        }
    }

    // ─────────────────────────────────────────────────────────────
    // Partial payments response (renegotiation)
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void PartialPaymentsResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("PaymentDunning/partial-payments-response.json");
        var response = new ResponseList<PaymentDunningPartialPayments>(System.Net.HttpStatusCode.OK, json);

        Assert.True(response.WasSuccessful());
        Assert.Single(response.Data);
        Assert.Equal(800m, response.Data[0].Value);
        Assert.Equal(new DateTime(2020, 2, 10), response.Data[0].PaymentDate);
    }

    // ─────────────────────────────────────────────────────────────
    // Payments available for dunning
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void PaymentsAvailableResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("PaymentDunning/payments-available-response.json");
        var response = new ResponseList<PaymentDunningPaymentAvailable>(System.Net.HttpStatusCode.OK, json);

        Assert.True(response.WasSuccessful());
        Assert.Single(response.Data);

        var item = response.Data[0];
        Assert.Equal("pay_856437540297", item.PaymentId);
        Assert.Equal("cus_000000001663", item.CustomerId);
        Assert.Equal(250m, item.Value);
        Assert.Equal(new DateTime(2020, 5, 18), item.DueDate);
        Assert.NotNull(item.TypeSimulations);
    }

    // ─────────────────────────────────────────────────────────────
    // Simulate response
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void SimulateResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("PaymentDunning/simulate-response.json");

        var result = JsonContractAssert.DeserializeFixture<SimulatedPaymentDunning>(json);

        Assert.Equal("pay_080225913252", result.PaymentId);
        Assert.Equal(80m, result.Value);
        // B-22m: typeSimulations e ARRAY no schema, nao objeto unico.
        Assert.Equal(2, result.TypeSimulations.Count);
        Assert.True(result.TypeSimulations[0].IsAllowed);
        Assert.False(result.TypeSimulations[1].IsAllowed);
        Assert.NotEmpty(result.TypeSimulations[1].NotAllowedReason);
    }

    // ─────────────────────────────────────────────────────────────
    // Create request - 9 campos obrigatorios + opcionais + documents
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void CreateRequest_UsesPaymentNotPaymentId()
    {
        // Regressao: campo deve serializar como "payment" (do schema),
        // nao "paymentId".
        var request = new CreatePaymentDunningRequest { PaymentId = "pay_x" };
        JsonContractAssert.SerializesWithKeys(request, "payment");
        JsonContractAssert.DoesNotSerializeKey(request, "paymentId");
    }

    // ─────────────────────────────────────────────────────────────
    // List filter
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void ListFilter_SerializesAllFieldsWithCorrectNames()
    {
        var filter = new PaymentDunningListFilter
        {
            Status = PaymentDunningStatus.PAID,
            Type = PaymentDunningType.CREDIT_BUREAU,
            PaymentId = "pay_1",
            RequestStartDate = new DateTime(2024, 1, 1),
            RequestEndDate = new DateTime(2024, 12, 31)
        };

        JsonContractAssert.QueryParamEquals(filter, "status", "PAID");
        JsonContractAssert.QueryParamEquals(filter, "type", "CREDIT_BUREAU");
        JsonContractAssert.QueryParamEquals(filter, "payment", "pay_1");
        JsonContractAssert.QueryParamEquals(filter, "requestStartDate", "2024-01-01");
        JsonContractAssert.QueryParamEquals(filter, "requestEndDate", "2024-12-31");
    }
}
