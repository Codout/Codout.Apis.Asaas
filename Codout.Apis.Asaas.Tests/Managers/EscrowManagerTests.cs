using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.Escrow;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class EscrowManagerTests : ManagerTestBase<EscrowManager>
{
    protected override EscrowManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableEscrowManager(settings, handler);

    [Fact]
    public async Task SaveSubaccountConfig_SendsPostToAccountsEscrowRoute()
    {
        SetupOkResponse("{\"enabled\":true,\"daysUntilExpire\":30}");
        var request = new SaveEscrowConfigRequest { Enabled = true, DaysUntilExpire = 30 };

        var result = await Manager.SaveSubaccountConfig("acc_1", request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/accounts/acc_1/escrow");
    }

    [Fact]
    public async Task GetSubaccountConfig_SendsGetToAccountsEscrowRoute()
    {
        SetupOkResponse("{\"enabled\":true}");

        var result = await Manager.GetSubaccountConfig("acc_1");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/accounts/acc_1/escrow");
    }

    [Fact]
    public async Task SaveDefaultConfig_SendsPostToAccountsEscrowRoot()
    {
        SetupOkResponse("{\"enabled\":true}");

        var result = await Manager.SaveDefaultConfig(new SaveEscrowConfigRequest { Enabled = true });

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/accounts/escrow");
    }

    [Fact]
    public async Task GetDefaultConfig_SendsGetToAccountsEscrowRoot()
    {
        SetupOkResponse("{\"enabled\":false}");

        var result = await Manager.GetDefaultConfig();

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/accounts/escrow");
    }

    [Fact]
    public async Task FinishPaymentEscrow_SendsPostToFinishRoute()
    {
        SetupOkResponse("{\"id\":\"esc_1\",\"status\":\"DONE\"}");

        var result = await Manager.FinishPaymentEscrow("esc_1", new FinishEscrowRequest { Reason = "MANUAL" });

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/escrow/esc_1/finish");
    }

    [Fact]
    public async Task GetPaymentEscrow_SendsGetToPaymentsEscrowRoute()
    {
        SetupOkResponse("{\"id\":\"esc_1\",\"status\":\"ACTIVE\"}");

        var result = await Manager.GetPaymentEscrow("pay_1");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/payments/pay_1/escrow");
    }

    [Fact]
    public async Task SaveSubaccountConfig_OnError_ReturnsErrorResponse()
    {
        SetupErrorResponse(HttpStatusCode.BadRequest);

        var result = await Manager.SaveSubaccountConfig("acc_1", new SaveEscrowConfigRequest());

        Assert.False(result.WasSuccessful());
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task GetPaymentEscrow_OnNotFound_ReturnsError()
    {
        SetupErrorResponse(HttpStatusCode.NotFound);

        var result = await Manager.GetPaymentEscrow("pay_unknown");

        Assert.False(result.WasSuccessful());
    }
}
