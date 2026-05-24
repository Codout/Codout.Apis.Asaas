using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.PixAutomatic;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class PixAutomaticManagerTests : ManagerTestBase<PixAutomaticManager>
{
    protected override PixAutomaticManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestablePixAutomaticManager(settings, handler);

    [Fact]
    public async Task CreateAuthorization_SendsPostToAuthorizationsRoute()
    {
        SetupOkResponse("{\"id\":\"auth_1\",\"status\":\"PENDING\"}");
        var request = new CreatePixAutomaticAuthorizationRequest { Customer = "cus_1", ContractId = "contract_1" };

        var result = await Manager.CreateAuthorization(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/pix/automatic/authorizations");
    }

    [Fact]
    public async Task ListAuthorizations_SendsGetToAuthorizationsRoute()
    {
        SetupListResponse<PixAutomaticAuthorization>("[]");

        var result = await Manager.ListAuthorizations(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/pix/automatic/authorizations");
    }

    [Fact]
    public async Task FindAuthorization_SendsGetToAuthorizationIdRoute()
    {
        SetupOkResponse("{\"id\":\"auth_1\"}");

        var result = await Manager.FindAuthorization("auth_1");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/pix/automatic/authorizations/auth_1");
    }

    [Fact]
    public async Task CancelAuthorization_SendsDeleteToAuthorizationIdRoute()
    {
        SetupOkResponse("{\"deleted\":true,\"id\":\"auth_1\"}");

        var result = await Manager.CancelAuthorization("auth_1");

        AssertRequestMethod(HttpMethod.Delete);
        AssertRequestUrl("/v3/pix/automatic/authorizations/auth_1");
    }

    [Fact]
    public async Task FindPaymentInstruction_SendsGetToPaymentInstructionIdRoute()
    {
        SetupOkResponse("{\"id\":\"pi_1\"}");

        var result = await Manager.FindPaymentInstruction("pi_1");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/pix/automatic/paymentInstructions/pi_1");
    }

    [Fact]
    public async Task ListPaymentInstructions_SendsGetToPaymentInstructionsRoute()
    {
        SetupListResponse<PixAutomaticPaymentInstruction>("[]");

        var result = await Manager.ListPaymentInstructions(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/pix/automatic/paymentInstructions");
    }
}
