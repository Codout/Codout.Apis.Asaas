using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.AsaasAccount;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class AsaasAccountManagerTests : ManagerTestBase<AsaasAccountManager>
{
    protected override AsaasAccountManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableAsaasAccountManager(settings, handler);

    // ── Create ──────────────────────────────────────────────────────

    [Fact]
    public async Task Create_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"name\":\"Test Company\",\"email\":\"test@test.com\",\"cpfCnpj\":\"12345678901\"}");

        var request = new CreateAccountRequest
        {
            Name = "Test Company",
            Email = "test@test.com",
            CpfCnpj = "12345678901"
        };

        var result = await Manager.Create(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/accounts");
    }

    [Fact]
    public async Task Create_DeserializesResponse()
    {
        SetupOkResponse("{\"name\":\"Test Company\",\"email\":\"test@test.com\",\"cpfCnpj\":\"12345678901\",\"phone\":\"1234567890\",\"mobilePhone\":\"0987654321\",\"address\":\"Rua Teste\",\"addressNumber\":\"123\",\"complement\":\"Apt 1\",\"province\":\"Centro\",\"postalCode\":\"12345678\",\"apiKey\":\"api_key_123\",\"walletId\":\"wallet_123\"}");

        var request = new CreateAccountRequest { Name = "Test Company", Email = "test@test.com", CpfCnpj = "12345678901" };

        var result = await Manager.Create(request);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("Test Company", result.Data.Name);
        Assert.Equal("test@test.com", result.Data.Email);
        Assert.Equal("12345678901", result.Data.CpfCnpj);
        Assert.Equal("1234567890", result.Data.Phone);
        Assert.Equal("0987654321", result.Data.MobilePhone);
        Assert.Equal("Rua Teste", result.Data.Address);
        Assert.Equal("123", result.Data.AddressNumber);
        Assert.Equal("Apt 1", result.Data.Complement);
        Assert.Equal("Centro", result.Data.Province);
        Assert.Equal("12345678", result.Data.PostalCode);
        Assert.Equal("api_key_123", result.Data.ApiKey);
        Assert.Equal("wallet_123", result.Data.WalletId);
    }

    // ── List ────────────────────────────────────────────────────────

    [Fact]
    public async Task List_SendsGetToCorrectUrl()
    {
        SetupListResponse<Account>("[]", totalCount: 0);

        var result = await Manager.List(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/accounts");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
    }

    [Fact]
    public async Task List_WithOffsetAndLimit_IncludesQueryParameters()
    {
        SetupListResponse<Account>("[]", totalCount: 0);

        var result = await Manager.List(5, 20);

        AssertRequestUrlContains("offset=5");
        AssertRequestUrlContains("limit=20");
    }

    [Fact]
    public async Task List_DeserializesResponse()
    {
        SetupListResponse<Account>("[{\"name\":\"Account 1\",\"email\":\"a1@test.com\",\"cpfCnpj\":\"111\"},{\"name\":\"Account 2\",\"email\":\"a2@test.com\",\"cpfCnpj\":\"222\"}]", totalCount: 2);

        var result = await Manager.List(0, 10);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal("Account 1", result.Data[0].Name);
        Assert.Equal("Account 2", result.Data[1].Name);
    }

    // ── Error handling ──────────────────────────────────────────────

    [Fact]
    public async Task Create_OnError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.BadRequest);

        var request = new CreateAccountRequest { Name = "Test", CpfCnpj = "invalid" };

        var result = await Manager.Create(request);

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("invalid", result.Errors[0].Code);
    }

    [Fact]
    public async Task List_OnError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.Forbidden);

        var result = await Manager.List(0, 10);

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        Assert.NotEmpty(result.Errors);
    }
}
