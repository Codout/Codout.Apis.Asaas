using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.MobilePhoneRecharge;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class MobilePhoneRechargeManagerTests : ManagerTestBase<MobilePhoneRechargeManager>
{
    protected override MobilePhoneRechargeManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableMobilePhoneRechargeManager(settings, handler);

    [Fact]
    public async Task Create_SendsPostToRechargesRoute()
    {
        SetupOkResponse("{\"id\":\"rec_1\",\"status\":\"PENDING\"}");
        var request = new CreateMobilePhoneRechargeRequest { PhoneNumber = "11999998888", Value = 20m };

        var result = await Manager.Create(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/mobilePhoneRecharges");
    }

    [Fact]
    public async Task List_SendsGetToRechargesRoute()
    {
        SetupListResponse<MobilePhoneRecharge>("[]");

        var result = await Manager.List(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/mobilePhoneRecharges");
    }

    [Fact]
    public async Task Find_SendsGetToRechargeIdRoute()
    {
        SetupOkResponse("{\"id\":\"rec_1\",\"status\":\"CONFIRMED\"}");

        var result = await Manager.Find("rec_1");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/mobilePhoneRecharges/rec_1");
    }

    [Fact]
    public async Task Cancel_SendsPostToCancelRoute()
    {
        SetupOkResponse("{\"id\":\"rec_1\",\"status\":\"CANCELLED\"}");

        var result = await Manager.Cancel("rec_1");

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/mobilePhoneRecharges/rec_1/cancel");
    }

    [Fact]
    public async Task GetProvider_SendsGetToProviderRoute()
    {
        SetupOkResponse("{\"name\":\"Vivo\",\"availableValues\":[10,20,30]}");

        var result = await Manager.GetProvider("11999998888");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/mobilePhoneRecharges/11999998888/provider");
        Assert.Equal("Vivo", result.Data.Name);
        Assert.Equal(3, result.Data.AvailableValues.Count);
    }

    [Fact]
    public async Task Find_DeserializesOperatorNameAndCanBeCancelled()
    {
        SetupOkResponse("{\"id\":\"rec_1\",\"value\":20.00,\"phoneNumber\":\"63997365512\",\"status\":\"CONFIRMED\",\"canBeCancelled\":true,\"operatorName\":\"Vivo\"}");

        var result = await Manager.Find("rec_1");

        Assert.True(result.WasSuccessful());
        Assert.Equal("Vivo", result.Data.OperatorName);
        Assert.True(result.Data.CanBeCancelled);
        Assert.Equal(Codout.Apis.Asaas.Models.MobilePhoneRecharge.Enums.MobilePhoneRechargeStatus.CONFIRMED, result.Data.Status);
    }
}
