using System;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.PixAutomatic;
using Codout.Apis.Asaas.Models.PixAutomatic.Enums;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class PixAutomaticManagerTests : ManagerTestBase<PixAutomaticManager>
{
    protected override PixAutomaticManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestablePixAutomaticManager(settings, handler);

    [Fact]
    public async Task CreateAuthorization_SendsPostToAuthorizationsRoute()
    {
        SetupOkResponse("{\"id\":\"auth_1\",\"status\":\"CREATED\",\"customerId\":\"cus_1\",\"frequency\":\"MONTHLY\"}");
        var request = new CreatePixAutomaticAuthorizationRequest
        {
            Frequency = PixAutomaticRecurringFrequency.MONTHLY,
            ContractId = "CONTRACT-123",
            StartDate = new DateTime(2026, 1, 1),
            CustomerId = "cus_1",
            Value = 100m,
            ImmediateQrCode = new CreatePixAutomaticImmediateQrCodeRequest
            {
                ExpirationSeconds = 3600,
                OriginalValue = 100m
            }
        };

        var result = await Manager.CreateAuthorization(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/pix/automatic/authorizations");
        Assert.Equal(PixAutomaticAuthorizationStatus.CREATED, result.Data.Status);
        Assert.Equal(PixAutomaticRecurringFrequency.MONTHLY, result.Data.Frequency);
    }

    [Fact]
    public async Task CreateAuthorization_SerializesImmediateQrCodeAndFrequency()
    {
        SetupOkResponse("{\"id\":\"auth_1\"}");
        var request = new CreatePixAutomaticAuthorizationRequest
        {
            Frequency = PixAutomaticRecurringFrequency.QUARTERLY,
            ContractId = "C-9",
            StartDate = new DateTime(2026, 2, 1),
            CustomerId = "cus_9",
            ImmediateQrCode = new CreatePixAutomaticImmediateQrCodeRequest { ExpirationSeconds = 600, OriginalValue = 50m }
        };

        await Manager.CreateAuthorization(request);

        Assert.NotNull(Handler.LastRequestContent);
        Assert.Contains("\"frequency\":\"QUARTERLY\"", Handler.LastRequestContent);
        Assert.Contains("\"immediateQrCode\":{", Handler.LastRequestContent);
        Assert.Contains("\"expirationSeconds\":600", Handler.LastRequestContent);
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
