using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.Common;
using Codout.Apis.Asaas.Models.MyAccount;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class MyAccountManagerTests : ManagerTestBase<MyAccountManager>
{
    protected override MyAccountManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableMyAccountManager(settings, handler);

    // ── Find ────────────────────────────────────────────────────────

    [Fact]
    public async Task Find_SendsGetToCorrectUrl()
    {
        SetupOkResponse("{\"name\":\"My Company\",\"email\":\"company@test.com\"}");

        var result = await Manager.Find();

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/myAccount");
    }

    [Fact]
    public async Task Find_DeserializesResponse()
    {
        SetupOkResponse("{\"name\":\"My Company\",\"email\":\"company@test.com\",\"cpfCnpj\":\"12345678901234\",\"phone\":\"1199998888\",\"mobilePhone\":\"11999887766\",\"address\":\"Rua Principal\",\"addressNumber\":\"500\",\"complement\":\"Sala 10\",\"province\":\"Centro\",\"postalCode\":\"01001000\",\"inscricaoEstadual\":\"123456789\",\"status\":\"ACTIVE\"}");

        var result = await Manager.Find();

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("My Company", result.Data.Name);
        Assert.Equal("company@test.com", result.Data.Email);
        Assert.Equal("12345678901234", result.Data.CpfCnpj);
        Assert.Equal("1199998888", result.Data.Phone);
        Assert.Equal("11999887766", result.Data.MobilePhone);
        Assert.Equal("Rua Principal", result.Data.Address);
        Assert.Equal("500", result.Data.AddressNumber);
        Assert.Equal("Sala 10", result.Data.Complement);
        Assert.Equal("Centro", result.Data.Province);
        Assert.Equal("01001000", result.Data.PostalCode);
        Assert.Equal("123456789", result.Data.InscricaoEstadual);
        Assert.Equal("ACTIVE", result.Data.Status);
    }

    // ── CreatePaymentCheckoutConfig ─────────────────────────────────

    [Fact]
    public async Task CreatePaymentCheckoutConfig_SendsPostToCorrectUrl()
    {
        SetupOkResponse("{\"logoBackgroundColor\":\"#FFFFFF\",\"infoBackgroundColor\":\"#000000\",\"fontColor\":\"#333333\",\"enabled\":true}");

        var request = new CreatePaymentCheckoutConfigRequest
        {
            LogoBackgroundColor = "#FFFFFF",
            InfoBackgroundColor = "#000000",
            FontColor = "#333333",
            Enabled = true,
            LogoFile = new AsaasFile { FileName = "logo.png", FileContent = new byte[] { 0x00 } }
        };

        var result = await Manager.CreatePaymentCheckoutConfig(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/myAccount/paymentCheckoutConfig");
    }

    [Fact]
    public async Task CreatePaymentCheckoutConfig_DeserializesResponse()
    {
        SetupOkResponse("{\"logoBackgroundColor\":\"#FFFFFF\",\"infoBackgroundColor\":\"#000000\",\"fontColor\":\"#333333\",\"enabled\":true,\"logoUrl\":\"https://example.com/logo.png\",\"observations\":\"Test obs\",\"status\":\"APPROVED\"}");

        var request = new CreatePaymentCheckoutConfigRequest
        {
            LogoBackgroundColor = "#FFFFFF",
            InfoBackgroundColor = "#000000",
            FontColor = "#333333",
            Enabled = true,
            LogoFile = new AsaasFile { FileName = "logo.png", FileContent = new byte[] { 0x00 } }
        };

        var result = await Manager.CreatePaymentCheckoutConfig(request);

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("#FFFFFF", result.Data.LogoBackgroundColor);
        Assert.Equal("#000000", result.Data.InfoBackgroundColor);
        Assert.Equal("#333333", result.Data.FontColor);
        Assert.True(result.Data.Enabled);
        Assert.Equal("https://example.com/logo.png", result.Data.LogoUrl);
        Assert.Equal("Test obs", result.Data.Observations);
        Assert.Equal("APPROVED", result.Data.Status);
    }

    // ── FindPaymentCheckoutConfig ───────────────────────────────────

    [Fact]
    public async Task FindPaymentCheckoutConfig_SendsGetToCorrectUrl()
    {
        SetupOkResponse("{\"logoBackgroundColor\":\"#FFFFFF\",\"enabled\":true}");

        var result = await Manager.FindPaymentCheckoutConfig();

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/myAccount/paymentCheckoutConfig");
    }

    [Fact]
    public async Task FindPaymentCheckoutConfig_DeserializesResponse()
    {
        SetupOkResponse("{\"logoBackgroundColor\":\"#FF0000\",\"infoBackgroundColor\":\"#00FF00\",\"fontColor\":\"#0000FF\",\"enabled\":false,\"logoUrl\":null,\"observations\":null,\"status\":\"PENDING\"}");

        var result = await Manager.FindPaymentCheckoutConfig();

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("#FF0000", result.Data.LogoBackgroundColor);
        Assert.False(result.Data.Enabled);
        Assert.Equal("PENDING", result.Data.Status);
    }

    // ── FindFees ────────────────────────────────────────────────────

    [Fact]
    public async Task FindFees_SendsGetToCorrectUrl()
    {
        SetupOkResponse("{\"payment\":{},\"transfer\":{},\"notification\":{},\"creditBureauReport\":{},\"invoice\":{},\"anticipation\":{}}");

        var result = await Manager.FindFees();

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/myAccount/fees");
    }

    [Fact]
    public async Task FindFees_DeserializesResponse()
    {
        SetupOkResponse("{\"payment\":{\"bankSlip\":{\"defaultValue\":3.50},\"creditCard\":{\"operationValue\":0.50}},\"transfer\":{\"monthlyTransfersWithoutFee\":10},\"notification\":{\"phoneCallFeeValue\":0.50},\"creditBureauReport\":{\"naturalPersonFeeValue\":12.00},\"invoice\":{\"feeValue\":5.00},\"anticipation\":{\"creditCard\":{\"operationValue\":1.00},\"bankSlip\":{\"defaultValue\":2.00}}}");

        var result = await Manager.FindFees();

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Payment);
        Assert.NotNull(result.Data.Transfer);
        Assert.NotNull(result.Data.Notification);
        Assert.NotNull(result.Data.Invoice);
        Assert.Equal(5.00m, result.Data.Invoice.FeeValue);
        Assert.Equal(10, result.Data.Transfer.MonthlyTransfersWithoutFee);
    }

    // ── FindAccountNumber ───────────────────────────────────────────

    [Fact]
    public async Task FindAccountNumber_SendsGetToCorrectUrl()
    {
        SetupOkResponse("{\"agency\":\"0001\",\"account\":\"123456\",\"accountDigit\":\"7\"}");

        var result = await Manager.FindAccountNumber();

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/myAccount/accountNumber");
    }

    [Fact]
    public async Task FindAccountNumber_DeserializesResponse()
    {
        SetupOkResponse("{\"agency\":\"0001\",\"account\":\"123456\",\"accountDigit\":\"7\"}");

        var result = await Manager.FindAccountNumber();

        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("0001", result.Data.Agency);
        Assert.Equal("123456", result.Data.Account);
        Assert.Equal("7", result.Data.AccountDigit);
    }

    // ── Error handling ──────────────────────────────────────────────

    [Fact]
    public async Task Find_OnError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.Unauthorized);

        var result = await Manager.Find();

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("invalid", result.Errors[0].Code);
    }

    [Fact]
    public async Task FindFees_OnError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.Forbidden);

        var result = await Manager.FindFees();

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        Assert.NotEmpty(result.Errors);
    }
}
