using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.Wallet;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class WalletManagerTests : ManagerTestBase<WalletManager>
{
    protected override WalletManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableWalletManager(settings, handler);

    #region List

    [Fact]
    public async Task List_SendsGetToCorrectUrl()
    {
        SetupListResponse<Wallet>("[{\"id\":\"wal_1\"}]");

        var result = await Manager.List(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/wallets");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    [Fact]
    public async Task List_DeserializesListResponseCorrectly()
    {
        SetupListResponse<Wallet>("[{\"id\":\"wal_1\"},{\"id\":\"wal_2\"},{\"id\":\"wal_3\"}]", totalCount: 3);

        var result = await Manager.List(0, 10);

        Assert.True(result.WasSucessfull());
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Data.Count);
        Assert.Equal("wal_1", result.Data[0].Id);
        Assert.Equal("wal_2", result.Data[1].Id);
        Assert.Equal("wal_3", result.Data[2].Id);
    }

    [Fact]
    public async Task List_WithPagination_IncludesOffsetAndLimit()
    {
        SetupListResponse<Wallet>("[]", totalCount: 0, limit: 20, offset: 5);

        var result = await Manager.List(5, 20);

        AssertRequestUrlContains("offset=5");
        AssertRequestUrlContains("limit=20");
    }

    [Fact]
    public async Task List_ReturnsEmptyListWhenNoWallets()
    {
        SetupListResponse<Wallet>("[]", totalCount: 0);

        var result = await Manager.List(0, 10);

        Assert.True(result.WasSucessfull());
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task List_HasMoreFlagIsParsedCorrectly()
    {
        SetupListResponse<Wallet>("[{\"id\":\"wal_1\"}]", totalCount: 50, limit: 10, offset: 0, hasMore: true);

        var result = await Manager.List(0, 10);

        Assert.True(result.WasSucessfull());
        Assert.True(result.HasMore);
        Assert.Equal(50, result.TotalCount);
        Assert.Equal(10, result.Limit);
        Assert.Equal(0, result.Offset);
    }

    [Fact]
    public async Task List_WhenApiReturnsError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.Unauthorized);

        var result = await Manager.List(0, 10);

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        Assert.NotEmpty(result.Errors);
    }

    #endregion
}
