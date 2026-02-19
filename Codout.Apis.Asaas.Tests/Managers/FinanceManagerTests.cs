using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.Finance;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class FinanceManagerTests : ManagerTestBase<FinanceManager>
{
    protected override FinanceManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableFinanceManager(settings, handler);

    #region Balance

    [Fact]
    public async Task Balance_SendsGetToCorrectUrl()
    {
        SetupOkResponse("12345.67");

        var result = await Manager.Balance();

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/finance/balance");
    }

    [Fact]
    public async Task Balance_DeserializesResponseCorrectly()
    {
        SetupOkResponse("12345.67");

        var result = await Manager.Balance();

        Assert.True(result.WasSucessfull());
        Assert.Equal(12345.67m, result.Data);
    }

    [Fact]
    public async Task Balance_WhenApiReturnsError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.Unauthorized);

        var result = await Manager.Balance();

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        Assert.NotEmpty(result.Errors);
    }

    #endregion

    #region ListTransactions

    [Fact]
    public async Task ListTransactions_SendsGetToCorrectUrl()
    {
        SetupListResponse<FinancialTransaction>("[{\"id\":\"txn_1\",\"value\":100.00}]");

        var result = await Manager.ListTransactions(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/financialTransactions");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    [Fact]
    public async Task ListTransactions_DeserializesListResponseCorrectly()
    {
        SetupListResponse<FinancialTransaction>("[{\"id\":\"txn_1\",\"value\":100.00,\"balance\":5000.00,\"type\":\"PAYMENT_FEE\",\"description\":\"Payment fee\"},{\"id\":\"txn_2\",\"value\":200.00,\"balance\":5200.00,\"type\":\"PAYMENT_RECEIVED\",\"description\":\"Payment received\"}]", totalCount: 2);

        var result = await Manager.ListTransactions(0, 10);

        Assert.True(result.WasSucessfull());
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("txn_1", result.Data[0].Id);
        Assert.Equal(100.00m, result.Data[0].Value);
        Assert.Equal(5000.00m, result.Data[0].Balance);
        Assert.Equal("PAYMENT_FEE", result.Data[0].Type);
        Assert.Equal("txn_2", result.Data[1].Id);
    }

    [Fact]
    public async Task ListTransactions_WithFilter_IncludesFilterParametersInUrl()
    {
        SetupListResponse<FinancialTransaction>("[{\"id\":\"txn_1\"}]");
        var filter = new FinancialTransactionListFilter
        {
            StartDate = new DateTime(2026, 1, 1),
            FinishDate = new DateTime(2026, 1, 31)
        };

        var result = await Manager.ListTransactions(0, 10, filter);

        AssertRequestUrlContains("startDate=");
        AssertRequestUrlContains("finishDate=");
    }

    [Fact]
    public async Task ListTransactions_WithPagination_IncludesOffsetAndLimit()
    {
        SetupListResponse<FinancialTransaction>("[]", totalCount: 0, limit: 20, offset: 5);

        var result = await Manager.ListTransactions(5, 20);

        AssertRequestUrlContains("offset=5");
        AssertRequestUrlContains("limit=20");
    }

    #endregion

    #region GetPaymentStatistics

    [Fact]
    public async Task GetPaymentStatistics_SendsGetToCorrectUrl()
    {
        SetupOkResponse("{\"quantity\":10,\"value\":1500.00,\"netValue\":1400.00}");

        var result = await Manager.GetPaymentStatistics();

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/finance/payment/statistics");
    }

    [Fact]
    public async Task GetPaymentStatistics_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"quantity\":10,\"value\":1500.00,\"netValue\":1400.00}");

        var result = await Manager.GetPaymentStatistics();

        Assert.True(result.WasSucessfull());
        Assert.Equal(10, result.Data.Quantity);
        Assert.Equal(1500.00m, result.Data.Value);
        Assert.Equal(1400.00m, result.Data.NetValue);
    }

    #endregion

    #region GetSplitStatistics

    [Fact]
    public async Task GetSplitStatistics_SendsGetToCorrectUrl()
    {
        SetupOkResponse("{\"totalPendingValue\":500.00,\"totalReceivedValue\":3000.00}");

        var result = await Manager.GetSplitStatistics();

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/finance/split/statistics");
    }

    [Fact]
    public async Task GetSplitStatistics_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"totalPendingValue\":500.00,\"totalReceivedValue\":3000.00}");

        var result = await Manager.GetSplitStatistics();

        Assert.True(result.WasSucessfull());
        Assert.Equal(500.00m, result.Data.TotalPendingValue);
        Assert.Equal(3000.00m, result.Data.TotalReceivedValue);
    }

    #endregion
}
