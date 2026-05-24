using Codout.Apis.Asaas.Models.Escrow;
using Codout.Apis.Asaas.Models.Escrow.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para EscrowManager (Conta de Garantia).
/// Schemas verificados via MCP em 2026-05-24:
/// - POST/GET /v3/accounts/{id}/escrow (subaccount config)
/// - POST/GET /v3/accounts/escrow (default config)
/// - POST /v3/escrow/{id}/finish (retorna Payment, body vazio)
/// - GET /v3/payments/{id}/escrow (PaymentEscrowGetResponseDTO)
/// Os 4 endpoints de config compartilham AccountPaymentEscrowConfigDTO.
/// </summary>
public class EscrowContractTests
{
    // ─────────────────────────────────────────────────────────────
    // Config endpoints (POST/GET subaccount, POST/GET default)
    // Schema: AccountPaymentEscrowConfigDTO { daysToExpire (required), enabled, isFeePayer }
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void SaveEscrowConfigRequest_HasCorrectFieldNames()
    {
        var request = new SaveEscrowConfigRequest
        {
            DaysToExpire = 30,
            Enabled = true,
            IsFeePayer = false
        };

        JsonContractAssert.SerializesWithKeys(request, "daysToExpire", "enabled", "isFeePayer");
    }

    [Fact]
    public void SaveEscrowConfigRequest_NoFakeFields()
    {
        var request = new SaveEscrowConfigRequest { DaysToExpire = 30 };

        // Regressao B-09: tinha DaysUntilExpire (errado) ao inves de DaysToExpire.
        JsonContractAssert.DoesNotSerializeKey(request, "daysUntilExpire");
    }

    [Fact]
    public void EscrowConfig_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("Escrow/config-response.json");

        var result = JsonContractAssert.DeserializeFixture<EscrowConfig>(json);

        Assert.Equal(30, result.DaysToExpire);
        Assert.True(result.Enabled);
        Assert.False(result.IsFeePayer);
    }

    [Fact]
    public void EscrowConfig_OptionalBoolsAreNullableInResponse()
    {
        // Apenas daysToExpire e required; enabled e isFeePayer podem vir omitidos.
        var json = "{\"daysToExpire\":30}";

        var result = JsonContractAssert.DeserializeFixture<EscrowConfig>(json);

        Assert.Equal(30, result.DaysToExpire);
        Assert.Null(result.Enabled);
        Assert.Null(result.IsFeePayer);
    }

    // ─────────────────────────────────────────────────────────────
    // GET /v3/payments/{id}/escrow -> PaymentEscrowGetResponseDTO
    // Schema: { id, status: enum(ACTIVE|DONE), expirationDate, finishDate, finishReason: enum }
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void PaymentEscrow_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("Escrow/payment-escrow-response.json");

        var result = JsonContractAssert.DeserializeFixture<Escrow>(json);

        Assert.Equal("4f468235-cec3-482f-b3d0-348af4c7194", result.Id);
        Assert.Equal(EscrowStatus.ACTIVE, result.Status);
        Assert.Equal(EscrowFinishReason.EXPIRED, result.FinishReason);
    }

    [Fact]
    public void EscrowStatus_BothValuesDeserialize()
    {
        foreach (var status in new[] { "ACTIVE", "DONE" })
        {
            var json = $"{{\"id\":\"x\",\"status\":\"{status}\"}}";
            var result = JsonContractAssert.DeserializeFixture<Escrow>(json);
            Assert.Equal(status, result.Status.ToString());
        }
    }

    [Fact]
    public void EscrowFinishReason_AllSixValuesDeserialize()
    {
        // Schema: CHARGEBACK, EXPIRED, INSUFFICIENT_BALANCE, PAYMENT_REFUNDED,
        //         REQUESTED_BY_CUSTOMER, CUSTOMER_CONFIG_DISABLED
        foreach (var reason in new[] {
            "CHARGEBACK", "EXPIRED", "INSUFFICIENT_BALANCE",
            "PAYMENT_REFUNDED", "REQUESTED_BY_CUSTOMER", "CUSTOMER_CONFIG_DISABLED" })
        {
            var json = $"{{\"id\":\"x\",\"status\":\"DONE\",\"finishReason\":\"{reason}\"}}";
            var result = JsonContractAssert.DeserializeFixture<Escrow>(json);
            Assert.NotNull(result.FinishReason);
            Assert.Equal(reason, result.FinishReason.ToString());
        }
    }

    [Fact]
    public void EscrowFinishReason_NullWhenStatusActive()
    {
        // Escrow ativo: finishReason vem null (so e preenchido quando status=DONE).
        var json = "{\"id\":\"esc_1\",\"status\":\"ACTIVE\"}";

        var result = JsonContractAssert.DeserializeFixture<Escrow>(json);

        Assert.Null(result.FinishReason);
    }
}
