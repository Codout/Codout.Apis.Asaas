using System;
using System.Text.Json;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.PixRecurring;
using Codout.Apis.Asaas.Models.PixRecurring.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para PixRecurringManager.
/// Schemas verificados via MCP em 2026-05-24:
/// - GET /v3/pix/transactions/recurrings (envelope padrao, filtros status/value/searchText)
/// - GET /v3/pix/transactions/recurrings/{id}
/// - POST /v3/pix/transactions/recurrings/{id}/cancel (body vazio, retorna transaction)
/// - GET /v3/pix/transactions/recurrings/{id}/items (envelope minimalista {data:[...]} sem hasMore)
/// - POST /v3/pix/transactions/recurrings/items/{id}/cancel (body vazio, retorna item)
/// </summary>
public class PixRecurringContractTests
{
    // ─────────────────────────────────────────────────────────────
    // PixRecurringTransaction response (B-12/B-13 regression coverage)
    // Schema: { id, status, origin, value, frequency, quantity,
    //           startDate, finishDate, canBeCancelled, externalAccount }
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void TransactionResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("PixRecurring/transaction-response.json");

        var result = JsonContractAssert.DeserializeFixture<PixRecurringTransaction>(json);

        Assert.Equal("35363f6e-93e2-11ec-b9d9-96f4053b1bd4", result.Id);
        Assert.Equal(PixRecurringStatus.PENDING, result.Status);
        Assert.Equal(PixRecurringOrigin.PIX, result.Origin);
        Assert.Equal(0.02m, result.Value);
        Assert.Equal(PixRecurringFrequency.WEEKLY, result.Frequency);
        Assert.Equal(2, result.Quantity);
        Assert.Equal(new DateTime(2024, 9, 18), result.StartDate);
        Assert.Equal(new DateTime(2024, 9, 25), result.FinishDate);
        Assert.True(result.CanBeCancelled);

        Assert.NotNull(result.ExternalAccount);
        Assert.Equal("John Doe", result.ExternalAccount.Name);
        Assert.Equal("Example bank S.A", result.ExternalAccount.FinancialInstitutionName);
        Assert.Equal("***.456.789-**", result.ExternalAccount.CpfCnpj);
        Assert.Equal("***.456.789-**", result.ExternalAccount.PixKey);
    }

    [Fact]
    public void TransactionsList_UsesStandardEnvelopeWithPagination()
    {
        // Diferente de /items que usa {data:[...]} sem paginacao, /recurrings
        // usa o envelope padrao com hasMore/totalCount/limit/offset.
        var json = FixtureLoader.Load("PixRecurring/transactions-list-response.json");

        var response = new ResponseList<PixRecurringTransaction>(System.Net.HttpStatusCode.OK, json);

        Assert.True(response.WasSuccessful());
        Assert.Equal(1, response.TotalCount);
        Assert.False(response.HasMore);
        Assert.Equal(10, response.Limit);
        Assert.Equal(0, response.Offset);
        Assert.Single(response.Data);
        Assert.Equal("35363f6e-93e2-11ec-b9d9-96f4053b1bd4", response.Data[0].Id);
    }

    [Fact]
    public void TransactionStatus_AllFiveValuesDeserialize()
    {
        // Schema: AWAITING_CRITICAL_ACTION_AUTHORIZATION, PENDING, SCHEDULED, CANCELLED, DONE
        foreach (var status in new[] {
            "AWAITING_CRITICAL_ACTION_AUTHORIZATION", "PENDING", "SCHEDULED", "CANCELLED", "DONE" })
        {
            var json = $"{{\"id\":\"x\",\"status\":\"{status}\"}}";
            var result = JsonContractAssert.DeserializeFixture<PixRecurringTransaction>(json);
            Assert.Equal(status, result.Status.ToString());
        }
    }

    [Fact]
    public void TransactionFrequency_BothValuesDeserialize()
    {
        foreach (var freq in new[] { "WEEKLY", "MONTHLY" })
        {
            var json = $"{{\"id\":\"x\",\"frequency\":\"{freq}\"}}";
            var result = JsonContractAssert.DeserializeFixture<PixRecurringTransaction>(json);
            Assert.NotNull(result.Frequency);
            Assert.Equal(freq, result.Frequency.ToString());
        }
    }

    [Fact]
    public void TransactionOrigin_PixDeserializes()
    {
        var json = "{\"id\":\"x\",\"origin\":\"PIX\"}";
        var result = JsonContractAssert.DeserializeFixture<PixRecurringTransaction>(json);
        Assert.NotNull(result.Origin);
        Assert.Equal(PixRecurringOrigin.PIX, result.Origin);
    }

    // ─────────────────────────────────────────────────────────────
    // PixRecurringItem response
    // Schema: { id, status, scheduledDate, canBeCancelled, recurrenceNumber,
    //           quantity, value, refusalReasonDescription, externalAccount }
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void ItemResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("PixRecurring/item-response.json");

        var result = JsonContractAssert.DeserializeFixture<PixRecurringItem>(json);

        Assert.Equal("71ae9d73-468f-4d04-8b87-a541128f9c46", result.Id);
        Assert.Equal(PixRecurringItemStatus.PENDING, result.Status);
        Assert.Equal(new DateTime(2024, 10, 23), result.ScheduledDate);
        Assert.True(result.CanBeCancelled);
        Assert.Equal(1, result.RecurrenceNumber);
        Assert.Equal(2, result.Quantity);
        Assert.Equal(0.02m, result.Value);
        Assert.Null(result.RefusalReasonDescription);

        Assert.NotNull(result.ExternalAccount);
        Assert.Equal("John Doe", result.ExternalAccount.Name);
    }

    [Fact]
    public void ItemStatus_AllFourValuesDeserialize()
    {
        // Schema: PENDING, CANCELLED, REFUSED, DONE
        foreach (var status in new[] { "PENDING", "CANCELLED", "REFUSED", "DONE" })
        {
            var json = $"{{\"id\":\"x\",\"status\":\"{status}\"}}";
            var result = JsonContractAssert.DeserializeFixture<PixRecurringItem>(json);
            Assert.Equal(status, result.Status.ToString());
        }
    }

    // ─────────────────────────────────────────────────────────────
    // Items envelope — B-14 regression: API retorna {data:[...]} sem
    // hasMore/totalCount/limit/offset, ao contrario de transactions list.
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void ItemsListEnvelope_UsesMinimalDataOnlyShape()
    {
        var json = FixtureLoader.Load("PixRecurring/items-list-envelope.json");

        JsonContractAssert.HasRootProperty(json, "data", JsonValueKind.Array);

        // Assert that the minimal envelope does NOT have pagination fields
        // (regressao B-14: codigo antigo tentava usar ResponseList aqui e quebrava)
        using var doc = JsonDocument.Parse(json);
        Assert.False(doc.RootElement.TryGetProperty("hasMore", out _));
        Assert.False(doc.RootElement.TryGetProperty("totalCount", out _));
        Assert.False(doc.RootElement.TryGetProperty("limit", out _));
        Assert.False(doc.RootElement.TryGetProperty("offset", out _));

        var wrapper = JsonContractAssert.DeserializeFixture<PixRecurringItemsResponse>(json);
        Assert.Single(wrapper.Data);
        Assert.Equal("71ae9d73-468f-4d04-8b87-a541128f9c46", wrapper.Data[0].Id);
    }

    // ─────────────────────────────────────────────────────────────
    // List filter — NOVO em Fase 3e
    // API espera: status (enum), value (number invariant), searchText (string)
    // Antes: List(offset,limit) nao aceitava filter algum.
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void TransactionListFilter_SerializesAllFieldsWithCorrectNames()
    {
        var filter = new PixRecurringTransactionListFilter
        {
            Status = PixRecurringStatus.SCHEDULED,
            Value = 12.5m,
            SearchText = "John"
        };

        JsonContractAssert.QueryParamEquals(filter, "status", "SCHEDULED");
        JsonContractAssert.QueryParamEquals(filter, "value", "12.5");
        JsonContractAssert.QueryParamEquals(filter, "searchText", "John");
    }

    [Fact]
    public void TransactionListFilter_DecimalUsesInvariantCulture()
    {
        // Em pt-BR, decimal.ToString() vira "12,5" sem cultura invariante.
        // RequestParameters.Add(decimal?) forca InvariantCulture (ponto, nao virgula).
        var prevCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
        try
        {
            System.Threading.Thread.CurrentThread.CurrentCulture =
                new System.Globalization.CultureInfo("pt-BR");

            var filter = new PixRecurringTransactionListFilter { Value = 99.99m };

            Assert.Equal("99.99", filter["value"]);
        }
        finally
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = prevCulture;
        }
    }

    [Fact]
    public void TransactionListFilter_StatusEnumSerializesAsUppercase()
    {
        var filter = new PixRecurringTransactionListFilter
        {
            Status = PixRecurringStatus.AWAITING_CRITICAL_ACTION_AUTHORIZATION
        };

        JsonContractAssert.QueryParamEquals(filter, "status", "AWAITING_CRITICAL_ACTION_AUTHORIZATION");
    }

    [Fact]
    public void TransactionListFilter_NullValuesAreOmitted()
    {
        var filter = new PixRecurringTransactionListFilter();

        Assert.False(filter.ContainsKey("status"));
        Assert.False(filter.ContainsKey("value"));
        Assert.False(filter.ContainsKey("searchText"));
    }
}
