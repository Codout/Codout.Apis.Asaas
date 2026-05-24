using System;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.PixAutomatic;
using Codout.Apis.Asaas.Models.PixAutomatic.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para PixAutomaticManager.
/// Schemas verificados via MCP em 2026-05-24.
/// </summary>
public class PixAutomaticContractTests
{
    // ─────────────────────────────────────────────────────────────
    // POST /v3/pix/automatic/authorizations
    // Required: frequency, contractId, startDate, customerId, immediateQrCode
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void CreateAuthorizationRequest_Minimal_HasRequiredFields()
    {
        var request = new CreatePixAutomaticAuthorizationRequest
        {
            Frequency = PixAutomaticRecurringFrequency.MONTHLY,
            ContractId = "CONTRACT-123",
            StartDate = new DateTime(2024, 1, 1),
            CustomerId = "cus_000005735721",
            ImmediateQrCode = new CreatePixAutomaticImmediateQrCodeRequest
            {
                ExpirationSeconds = 3600,
                OriginalValue = 100m
            }
        };

        JsonContractAssert.SerializesWithKeys(request,
            "frequency", "contractId", "startDate", "customerId", "immediateQrCode");
    }

    [Fact]
    public void CreateAuthorizationRequest_NoFakeFields()
    {
        var request = new CreatePixAutomaticAuthorizationRequest
        {
            Frequency = PixAutomaticRecurringFrequency.MONTHLY,
            ContractId = "x",
            StartDate = new DateTime(2026, 1, 1),
            CustomerId = "cus_x",
            ImmediateQrCode = new CreatePixAutomaticImmediateQrCodeRequest { ExpirationSeconds = 60, OriginalValue = 1m }
        };

        // Regressao B-06: havia FixedValue, MaximumValue, Periodicity, ExpirationDate, Customer (sem Id)
        JsonContractAssert.DoesNotSerializeKey(request, "fixedValue");
        JsonContractAssert.DoesNotSerializeKey(request, "maximumValue");
        JsonContractAssert.DoesNotSerializeKey(request, "periodicity");
        JsonContractAssert.DoesNotSerializeKey(request, "expirationDate");
        JsonContractAssert.DoesNotSerializeKey(request, "customer");
    }

    [Fact]
    public void AuthorizationResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("PixAutomatic/authorization-response.json");

        var result = JsonContractAssert.DeserializeFixture<PixAutomaticAuthorization>(json);

        Assert.Equal("a33047b1-fb19-4b68-9373-a7ba8a8162aa", result.Id);
        Assert.Equal(50m, result.MinLimitValue);
        Assert.Null(result.CancellationDate);
        Assert.Equal("XXXYYYY1234", result.ContractId);
        Assert.Equal("cus_000005735721", result.CustomerId);
        Assert.Equal("Music and Movie Streaming Services", result.Description);
        Assert.Equal(new DateTime(2024, 12, 31), result.FinishDate);
        Assert.Equal(PixAutomaticRecurringFrequency.MONTHLY, result.Frequency);
        Assert.Equal("RR1234567820240115abcdefghijk", result.EndToEndIdentifier);
        Assert.Equal(new DateTime(2024, 1, 1), result.StartDate);
        Assert.Equal(PixAutomaticAuthorizationStatus.ACTIVE, result.Status);
        Assert.Equal(100m, result.Value);

        Assert.NotNull(result.ImmediateQrCode);
        Assert.Equal("E12345678202401011234567890123456", result.ImmediateQrCode.ConciliationIdentifier);

        Assert.Equal(PixAutomaticOriginType.IMMEDIATE_PAYMENT_AND_RECURRING_QR_CODE, result.OriginType);
        Assert.Equal("sub_000005735721", result.SubscriptionId);
    }

    [Fact]
    public void AuthorizationsListResponse_UsesStandardEnvelopeWithPagination()
    {
        // Diferente de PixRecurring/items e MyAccount/documents que usam {data:[...]},
        // este endpoint usa o envelope padrao com hasMore/totalCount/limit/offset.
        var json = FixtureLoader.Load("PixAutomatic/authorizations-list-response.json");

        var response = new ResponseList<PixAutomaticAuthorization>(System.Net.HttpStatusCode.OK, json);

        Assert.True(response.WasSuccessful());
        Assert.Equal(1, response.TotalCount);
        Assert.False(response.HasMore);
        Assert.Equal(10, response.Limit);
        Assert.Equal(0, response.Offset);
        Assert.Single(response.Data);
        Assert.Equal("a33047b1-fb19-4b68-9373-a7ba8a8162aa", response.Data[0].Id);
    }

    [Fact]
    public void AuthorizationStatus_AllFiveValuesDeserialize()
    {
        foreach (var status in new[] { "CREATED", "ACTIVE", "CANCELLED", "REFUSED", "EXPIRED" })
        {
            var json = $"{{\"id\":\"x\",\"status\":\"{status}\"}}";
            var result = JsonContractAssert.DeserializeFixture<PixAutomaticAuthorization>(json);
            Assert.Equal(status, result.Status.ToString());
        }
    }

    [Fact]
    public void AuthorizationFrequency_AllFiveValuesDeserialize()
    {
        foreach (var freq in new[] { "WEEKLY", "MONTHLY", "QUARTERLY", "SEMIANNUALLY", "ANNUALLY" })
        {
            var json = $"{{\"id\":\"x\",\"frequency\":\"{freq}\"}}";
            var result = JsonContractAssert.DeserializeFixture<PixAutomaticAuthorization>(json);
            Assert.NotNull(result.Frequency);
            Assert.Equal(freq, result.Frequency.ToString());
        }
    }

    // ─────────────────────────────────────────────────────────────
    // PaymentInstruction (B-16 regression coverage)
    // Schema: { id, endToEndIdentifier, authorization: {id, e2e, customerId},
    //           dueDate, status enum, paymentId, refusalReason }
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void PaymentInstruction_DeserializesFromOfficialFixture_WithNestedAuthorization()
    {
        var json = FixtureLoader.Load("PixAutomatic/payment-instruction-response.json");

        var result = JsonContractAssert.DeserializeFixture<PixAutomaticPaymentInstruction>(json);

        Assert.Equal("d1c9c4b2-6a97-4573-9d6d-26bb64f04c28", result.Id);
        Assert.Equal("E00416968202111161635q5bk0brYk2C", result.EndToEndIdentifier);
        Assert.Equal(new DateTime(2020, 1, 31), result.DueDate);
        Assert.Equal(PixAutomaticPaymentInstructionStatus.SCHEDULED, result.Status);
        Assert.Equal("pay_tsp88gie3b5e6o2p", result.PaymentId);
        Assert.Null(result.RefusalReason);

        Assert.NotNull(result.Authorization);
        Assert.Equal("35363f6e-93e2-11ec-b9d9-96f4053b1bd4", result.Authorization.Id);
        Assert.Equal("RR1234567820240115abcdefghijk", result.Authorization.EndToEndIdentifier);
        Assert.Equal("cus_000005735721", result.Authorization.CustomerId);
    }

    [Fact]
    public void PaymentInstructionStatus_AllFiveValuesDeserialize()
    {
        foreach (var status in new[] { "AWAITING_REQUEST", "SCHEDULED", "DONE", "CANCELLED", "REFUSED" })
        {
            var json = $"{{\"id\":\"x\",\"status\":\"{status}\"}}";
            var result = JsonContractAssert.DeserializeFixture<PixAutomaticPaymentInstruction>(json);
            Assert.Equal(status, result.Status.ToString());
        }
    }

    // ─────────────────────────────────────────────────────────────
    // PaymentInstructionListFilter — regression B-17
    // API espera: authorizationId, customerId, paymentId, status
    // Antes: authorization, status (errado)
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void PaymentInstructionListFilter_SerializesAuthorizationIdNotAuthorization()
    {
        var filter = new PixAutomaticPaymentInstructionListFilter
        {
            AuthorizationId = "auth_1",
            CustomerId = "cus_1",
            PaymentId = "pay_1",
            Status = PixAutomaticPaymentInstructionStatus.SCHEDULED
        };

        JsonContractAssert.QueryParamEquals(filter, "authorizationId", "auth_1");
        JsonContractAssert.QueryParamEquals(filter, "customerId", "cus_1");
        JsonContractAssert.QueryParamEquals(filter, "paymentId", "pay_1");
        JsonContractAssert.QueryParamEquals(filter, "status", "SCHEDULED");
        Assert.False(filter.ContainsKey("authorization"));
    }
}
