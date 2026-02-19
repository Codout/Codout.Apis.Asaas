using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.CreditBureauReport;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class CreditBureauReportManagerTests : ManagerTestBase<CreditBureauReportManager>
{
    protected override CreditBureauReportManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableCreditBureauReportManager(settings, handler);

    #region Create

    [Fact]
    public async Task Create_SendsPostRequest()
    {
        SetupOkResponse("{\"id\":\"cbr_123\",\"customer\":\"cus_abc\",\"cpfCnpj\":\"12345678901\",\"state\":\"SP\",\"status\":\"PENDING\",\"dateCreated\":\"2024-01-15\"}");

        var request = new CreateCreditBureauReportRequest
        {
            Customer = "cus_abc",
            CpfCnpj = "12345678901",
            State = "SP"
        };

        var result = await Manager.Create(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/creditBureauReport");
        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("cbr_123", result.Data.Id);
        Assert.Equal("cus_abc", result.Data.Customer);
        Assert.Equal("12345678901", result.Data.CpfCnpj);
        Assert.Equal("SP", result.Data.State);
        Assert.Equal("PENDING", result.Data.Status);
    }

    [Fact]
    public async Task Create_SerializesRequestBody()
    {
        SetupOkResponse("{\"id\":\"cbr_123\"}");

        var request = new CreateCreditBureauReportRequest
        {
            Customer = "cus_test",
            CpfCnpj = "98765432100",
            State = "RJ"
        };

        await Manager.Create(request);

        Assert.NotNull(Handler.LastRequestContent);
        Assert.Contains("\"customer\":\"cus_test\"", Handler.LastRequestContent);
        Assert.Contains("\"cpfCnpj\":\"98765432100\"", Handler.LastRequestContent);
        Assert.Contains("\"state\":\"RJ\"", Handler.LastRequestContent);
    }

    #endregion

    #region List

    [Fact]
    public async Task List_SendsGetRequest()
    {
        SetupListResponse<CreditBureauReport>("[{\"id\":\"cbr_1\",\"customer\":\"cus_abc\",\"cpfCnpj\":\"12345678901\",\"state\":\"SP\",\"status\":\"DONE\",\"dateCreated\":\"2024-01-15\"}]", totalCount: 1, limit: 10, offset: 0);

        var result = await Manager.List(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/creditBureauReport");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
        Assert.True(result.WasSucessfull());
        Assert.Single(result.Data);
        Assert.Equal("cbr_1", result.Data[0].Id);
        Assert.Equal("DONE", result.Data[0].Status);
    }

    [Fact]
    public async Task List_RespectsOffsetAndLimit()
    {
        SetupListResponse<CreditBureauReport>("[]", totalCount: 0, limit: 5, offset: 10);

        var result = await Manager.List(10, 5);

        AssertRequestUrlContains("offset=10");
        AssertRequestUrlContains("limit=5");
        Assert.Empty(result.Data);
        Assert.Equal(0, result.TotalCount);
    }

    #endregion

    #region Find

    [Fact]
    public async Task Find_SendsGetRequestWithId()
    {
        SetupOkResponse("{\"id\":\"cbr_456\",\"customer\":\"cus_def\",\"cpfCnpj\":\"11122233344\",\"state\":\"MG\",\"status\":\"DONE\",\"dateCreated\":\"2024-02-20\"}");

        var result = await Manager.Find("cbr_456");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/creditBureauReport/cbr_456");
        Assert.True(result.WasSucessfull());
        Assert.Equal("cbr_456", result.Data.Id);
        Assert.Equal("cus_def", result.Data.Customer);
        Assert.Equal("MG", result.Data.State);
    }

    #endregion

    #region Error Handling

    [Fact]
    public async Task Create_ReturnsErrorOnBadRequest()
    {
        SetupErrorResponse(HttpStatusCode.BadRequest);

        var request = new CreateCreditBureauReportRequest { Customer = "cus_invalid" };
        var result = await Manager.Create(request);

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("invalid", result.Errors[0].Code);
    }

    [Fact]
    public async Task Find_ReturnsErrorOnNotFound()
    {
        Handler.WithResponse(HttpStatusCode.NotFound, "{\"errors\":[{\"code\":\"not_found\",\"description\":\"Report not found\"}]}");

        var result = await Manager.Find("cbr_nonexistent");

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.Single(result.Errors);
        Assert.Equal("not_found", result.Errors[0].Code);
    }

    [Fact]
    public async Task Find_ReturnsNullDataOnError()
    {
        SetupErrorResponse(HttpStatusCode.BadRequest);

        var result = await Manager.Find("cbr_bad");

        Assert.Null(result.Data);
    }

    #endregion
}
