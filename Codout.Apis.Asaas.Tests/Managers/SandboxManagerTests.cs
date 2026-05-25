using System;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class SandboxManagerTests : ManagerTestBase<SandboxManager>
{
    protected override SandboxManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableSandboxManager(settings, handler);

    [Fact]
    public async Task ApproveAccount_SendsPostToApproveRoute()
    {
        SetupOkResponse("{}");

        var result = await Manager.ApproveAccount();

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/sandbox/myAccount/approve");
    }

    [Fact]
    public async Task ConfirmPayment_SendsPostToConfirmRoute()
    {
        SetupOkResponse("{\"id\":\"pay_1\",\"status\":\"CONFIRMED\"}");

        var result = await Manager.ConfirmPayment("pay_1");

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/sandbox/payment/pay_1/confirm");
    }

    [Fact]
    public async Task ForceOverdue_SendsPostToOverdueRoute()
    {
        SetupOkResponse("{\"id\":\"pay_1\",\"status\":\"OVERDUE\"}");

        var result = await Manager.ForceOverdue("pay_1");

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/sandbox/payment/pay_1/overdue");
    }

    [Fact]
    public async Task ApproveAccount_InProduction_ThrowsInvalidOperationException()
    {
        var prodSettings = new ApiSettings("token", "TestApp", AsaasEnvironment.PRODUCTION);
        var prodManager = new TestableSandboxManager(prodSettings, Handler);

        await Assert.ThrowsAsync<InvalidOperationException>(() => prodManager.ApproveAccount());
    }

    [Fact]
    public async Task ConfirmPayment_InProduction_ThrowsInvalidOperationException()
    {
        var prodSettings = new ApiSettings("token", "TestApp", AsaasEnvironment.PRODUCTION);
        var prodManager = new TestableSandboxManager(prodSettings, Handler);

        await Assert.ThrowsAsync<InvalidOperationException>(() => prodManager.ConfirmPayment("pay_1"));
    }

    [Fact]
    public async Task ForceOverdue_InProduction_ThrowsInvalidOperationException()
    {
        var prodSettings = new ApiSettings("token", "TestApp", AsaasEnvironment.PRODUCTION);
        var prodManager = new TestableSandboxManager(prodSettings, Handler);

        await Assert.ThrowsAsync<InvalidOperationException>(() => prodManager.ForceOverdue("pay_1"));
    }
}
