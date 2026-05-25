using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.Chargeback;
using Codout.Apis.Asaas.Models.Chargeback.Enums;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class ChargebackManagerTests : ManagerTestBase<ChargebackManager>
{
    protected override ChargebackManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableChargebackManager(settings, handler);

    [Fact]
    public async Task List_SendsGetToChargebacksRoute()
    {
        SetupListResponse<Chargeback>("[{\"id\":\"chrg_1\",\"payment\":\"pay_1\",\"status\":\"REQUESTED\",\"reason\":\"FRAUD\",\"value\":100}]", totalCount: 1);

        var result = await Manager.List(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/chargebacks");
        Assert.True(result.WasSuccessful());
        Assert.Equal("chrg_1", result.Data[0].Id);
        Assert.Equal(ChargebackStatus.REQUESTED, result.Data[0].Status);
        Assert.Equal(ChargebackReason.FRAUD, result.Data[0].Reason);
    }

    [Fact]
    public async Task FindByPayment_SendsGetToPaymentChargebackRoute()
    {
        SetupOkResponse("{\"id\":\"chrg_1\",\"payment\":\"pay_1\",\"status\":\"DONE\",\"reason\":\"COMMERCIAL_DISAGREEMENT\",\"value\":50}");

        var result = await Manager.FindByPayment("pay_1");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/payments/pay_1/chargeback");
        Assert.True(result.WasSuccessful());
        Assert.Equal("chrg_1", result.Data.Id);
        Assert.Equal(ChargebackStatus.DONE, result.Data.Status);
    }

    [Fact]
    public async Task CreateDispute_SendsPostToDisputeRoute()
    {
        SetupOkResponse("{\"id\":\"chrg_1\",\"disputeStatus\":\"REQUESTED\"}");

        var request = new CreateChargebackDisputeRequest { Description = "Tenho prova de entrega" };

        var result = await Manager.CreateDispute("chrg_1", request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/chargebacks/chrg_1/dispute");
    }
}
