using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.Transfer;
using Codout.Apis.Asaas.Models.Transfer.Base;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class TransferManagerTests : ManagerTestBase<TransferManager>
{
    protected override TransferManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableTransferManager(settings, handler);

    #region List

    [Fact]
    public async Task List_SendsGetToCorrectUrl()
    {
        SetupListResponse<BaseTransfer>("[]", totalCount: 0);

        var result = await Manager.List(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/transfers");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    [Fact]
    public async Task List_DeserializesListResponseCorrectly()
    {
        SetupListResponse<BaseTransfer>("[]", totalCount: 0);

        var result = await Manager.List(0, 10);

        Assert.True(result.WasSucessfull());
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task List_WithFilter_IncludesFilterParametersInUrl()
    {
        SetupListResponse<BaseTransfer>("[]", totalCount: 0);
        var filter = new TransferListFilter
        {
            TransferType = Models.Transfer.Enums.TransferType.BANK_ACCOUNT
        };

        var result = await Manager.List(0, 10, filter);

        AssertRequestUrlContains("type=BANK_ACCOUNT");
    }

    [Fact]
    public async Task List_WithPagination_IncludesOffsetAndLimit()
    {
        SetupListResponse<BaseTransfer>("[]", totalCount: 0, limit: 20, offset: 5);

        var result = await Manager.List(5, 20);

        AssertRequestUrlContains("offset=5");
        AssertRequestUrlContains("limit=20");
    }

    [Fact]
    public async Task List_WhenApiReturnsError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.Forbidden);

        var result = await Manager.List(0, 10);

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        Assert.NotEmpty(result.Errors);
    }

    #endregion

    #region Execute (AsaasAccountTransfer)

    [Fact]
    public async Task ExecuteAsaasAccountTransfer_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"trans_123\",\"walletId\":\"wal_456\",\"value\":500.00,\"type\":\"ASAAS_ACCOUNT\",\"status\":\"PENDING\"}");
        var request = new AsaasAccountTransferRequest
        {
            WalletId = "wal_456",
            Value = 500.00m
        };

        var result = await Manager.Execute(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/transfers");
    }

    [Fact]
    public async Task ExecuteAsaasAccountTransfer_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"trans_123\",\"walletId\":\"wal_456\",\"value\":500.00,\"type\":\"ASAAS_ACCOUNT\",\"status\":\"PENDING\",\"authorized\":true}");
        var request = new AsaasAccountTransferRequest
        {
            WalletId = "wal_456",
            Value = 500.00m
        };

        var result = await Manager.Execute(request);

        Assert.True(result.WasSucessfull());
        Assert.Equal("trans_123", result.Data.Id);
        Assert.Equal("wal_456", result.Data.WalletId);
        Assert.Equal(500.00m, result.Data.Value);
    }

    #endregion

    #region Execute (BankAccountTransfer)

    [Fact]
    public async Task ExecuteBankAccountTransfer_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"id\":\"trans_789\",\"value\":1000.00,\"type\":\"BANK_ACCOUNT\",\"status\":\"PENDING\"}");
        var request = new BankAccountTransferRequest
        {
            Value = 1000.00m,
            BankAccount = new BankAccount()
        };

        var result = await Manager.Execute(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/transfers");
    }

    [Fact]
    public async Task ExecuteBankAccountTransfer_DeserializesResponseCorrectly()
    {
        SetupOkResponse("{\"id\":\"trans_789\",\"value\":1000.00,\"netValue\":995.00,\"type\":\"BANK_ACCOUNT\",\"status\":\"PENDING\"}");
        var request = new BankAccountTransferRequest
        {
            Value = 1000.00m,
            BankAccount = new BankAccount()
        };

        var result = await Manager.Execute(request);

        Assert.True(result.WasSucessfull());
        Assert.Equal("trans_789", result.Data.Id);
        Assert.Equal(1000.00m, result.Data.Value);
    }

    #endregion

    #region Find

    [Fact]
    public async Task Find_SendsGetToCorrectUrl()
    {
        SetupErrorResponse(HttpStatusCode.NotFound);

        var result = await Manager.Find("trans_123");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/transfers/trans_123");
    }

    [Fact]
    public async Task Find_DeserializesResponseCorrectly()
    {
        // BaseTransfer is abstract and cannot be deserialized by System.Text.Json.
        // Verify that attempting to deserialize an abstract type throws NotSupportedException.
        SetupOkResponse("{\"id\":\"trans_123\",\"value\":1000.00,\"type\":\"BANK_ACCOUNT\",\"transferFee\":5.00,\"authorized\":true}");

        await Assert.ThrowsAsync<NotSupportedException>(() => Manager.Find("trans_123"));
    }

    [Fact]
    public async Task Find_WhenNotFound_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.NotFound);

        var result = await Manager.Find("trans_nonexistent");

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion
}
