using System;
using System.Text.Json;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.CreditBureauReport;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para CreditBureauReportManager.
/// Schemas verificados via MCP em 2026-05-24:
/// - POST /v3/creditBureauReport (request {customer?, cpfCnpj?})
/// - GET /v3/creditBureauReport (envelope padrao + filtros startDate/endDate)
/// - GET /v3/creditBureauReport/{id}
/// </summary>
public class CreditBureauReportContractTests
{
    // ─────────────────────────────────────────────────────────────
    // POST request - schema CreditBureauReportSaveRequestDTO
    // Apenas {customer, cpfCnpj} (ambos opcionais).
    // B-23d: o modelo antigo tinha State (nao existe no schema).
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void CreateRequest_HasOnlyCustomerAndCpfCnpj()
    {
        var request = new CreateCreditBureauReportRequest
        {
            Customer = "cus_000000001766",
            CpfCnpj = "05666663755"
        };

        JsonContractAssert.SerializesWithKeys(request, "customer", "cpfCnpj");
    }

    [Fact]
    public void CreateRequest_DoesNotSerializeRemovedFields()
    {
        // Regressao B-23d: o campo State NAO existe no schema oficial.
        var request = new CreateCreditBureauReportRequest { Customer = "cus_x" };

        JsonContractAssert.DoesNotSerializeKey(request, "state");
        JsonContractAssert.DoesNotSerializeKey(request, "status");
    }

    // ─────────────────────────────────────────────────────────────
    // Response shape - schema CreditBureauReportGetResponseDTO
    // Fields: id, dateCreated, cpfCnpj, customer, downloadUrl, reportFile
    // B-23a/b: model antigo tinha State e Status (nao existem no schema)
    // B-23c: model antigo nao tinha DownloadUrl nem ReportFile
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void ReportResponse_DeserializesFromOfficialFixture_GetById()
    {
        var json = FixtureLoader.Load("CreditBureauReport/report-response.json");

        var result = JsonContractAssert.DeserializeFixture<CreditBureauReport>(json);

        Assert.Equal("6c5e73fa-9efd-4a75-b60c-1cafb8d1c7ed", result.Id);
        Assert.Equal(new DateTime(2025, 5, 30), result.DateCreated);
        Assert.Equal("05666663755", result.CpfCnpj);
        Assert.Equal("cus_000000001766", result.Customer);
        Assert.Equal("https://www.asaas.com.br/creditBureauReport/download/6c5e73fa-9efd-4a75-b60c-1cafb8d1c7ed", result.DownloadUrl);
        // Em GET por id e itens do List, reportFile vem null.
        Assert.Null(result.ReportFile);
    }

    [Fact]
    public void ReportResponse_PopulatesReportFile_OnCreate()
    {
        // Schema documenta que reportFile (PDF Base64) e retornado APENAS no POST.
        var json = FixtureLoader.Load("CreditBureauReport/report-create-response.json");

        var result = JsonContractAssert.DeserializeFixture<CreditBureauReport>(json);

        Assert.NotNull(result.ReportFile);
        Assert.StartsWith("JVBERi", result.ReportFile);
    }

    [Fact]
    public void ReportResponse_NoFakeFields()
    {
        // Regressao B-23a/b: schema NAO retorna State nem Status.
        var json = FixtureLoader.Load("CreditBureauReport/report-response.json");

        using var doc = JsonDocument.Parse(json);
        Assert.False(doc.RootElement.TryGetProperty("state", out _));
        Assert.False(doc.RootElement.TryGetProperty("status", out _));
    }

    [Fact]
    public void ReportsList_UsesStandardEnvelopeWithPagination()
    {
        var json = FixtureLoader.Load("CreditBureauReport/reports-list-response.json");

        var response = new ResponseList<CreditBureauReport>(System.Net.HttpStatusCode.OK, json);

        Assert.True(response.WasSuccessful());
        Assert.Equal(1, response.TotalCount);
        Assert.False(response.HasMore);
        Assert.Single(response.Data);
        Assert.Equal("6c5e73fa-9efd-4a75-b60c-1cafb8d1c7ed", response.Data[0].Id);
    }

    // ─────────────────────────────────────────────────────────────
    // List filter - B-23e
    // Schema expoe startDate e endDate. Antes nao existia filter.
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void ListFilter_SerializesDateFieldsAsIso()
    {
        var filter = new CreditBureauReportListFilter
        {
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 12, 31)
        };

        JsonContractAssert.QueryParamEquals(filter, "startDate", "2024-01-01");
        JsonContractAssert.QueryParamEquals(filter, "endDate", "2024-12-31");
    }

    [Fact]
    public void ListFilter_NullValuesAreOmitted()
    {
        var filter = new CreditBureauReportListFilter();

        Assert.False(filter.ContainsKey("startDate"));
        Assert.False(filter.ContainsKey("endDate"));
    }
}
