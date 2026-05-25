using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.MobilePhoneRecharge;
using Codout.Apis.Asaas.Models.MobilePhoneRecharge.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para MobilePhoneRechargeManager.
/// Schemas verificados via MCP em 2026-05-24:
/// - POST /v3/mobilePhoneRecharges (request: value+phoneNumber required)
/// - GET /v3/mobilePhoneRecharges (envelope padrao)
/// - GET /v3/mobilePhoneRecharges/{id}
/// - POST /v3/mobilePhoneRecharges/{id}/cancel (body vazio)
/// - GET /v3/mobilePhoneRecharges/{phoneNumber}/provider (B-19 regression)
/// </summary>
public class MobilePhoneRechargeContractTests
{
    // ─────────────────────────────────────────────────────────────
    // POST /v3/mobilePhoneRecharges request
    // Required: value, phoneNumber
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void CreateRequest_HasRequiredFields()
    {
        var request = new CreateMobilePhoneRechargeRequest
        {
            Value = 15m,
            PhoneNumber = "63997365512"
        };

        JsonContractAssert.SerializesWithKeys(request, "value", "phoneNumber");
    }

    [Fact]
    public void CreateRequest_NoFakeFields()
    {
        var request = new CreateMobilePhoneRechargeRequest { Value = 15m, PhoneNumber = "63997365512" };

        // Schema oficial expoe APENAS value e phoneNumber. Nada de description,
        // operator, customer etc.
        JsonContractAssert.DoesNotSerializeKey(request, "description");
        JsonContractAssert.DoesNotSerializeKey(request, "operator");
        JsonContractAssert.DoesNotSerializeKey(request, "customer");
    }

    // ─────────────────────────────────────────────────────────────
    // GET /v3/mobilePhoneRecharges/{id} -> MobilePhoneRechargeGetResponseDTO
    // Schema: { id, value, phoneNumber, status enum, canBeCancelled, operatorName }
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void RechargeResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("MobilePhoneRecharge/recharge-response.json");

        var result = JsonContractAssert.DeserializeFixture<MobilePhoneRecharge>(json);

        Assert.Equal("37c22147-4194-11ec-8061-0242ac120002", result.Id);
        Assert.Equal(15m, result.Value);
        Assert.Equal("63997365512", result.PhoneNumber);
        Assert.Equal(MobilePhoneRechargeStatus.PENDING, result.Status);
        Assert.True(result.CanBeCancelled);
        Assert.Equal("Vivo", result.OperatorName);
    }

    [Fact]
    public void RechargesList_UsesStandardEnvelopeWithPagination()
    {
        var json = FixtureLoader.Load("MobilePhoneRecharge/recharges-list-response.json");

        var response = new ResponseList<MobilePhoneRecharge>(System.Net.HttpStatusCode.OK, json);

        Assert.True(response.WasSuccessful());
        Assert.Equal(1, response.TotalCount);
        Assert.False(response.HasMore);
        Assert.Equal(10, response.Limit);
        Assert.Equal(0, response.Offset);
        Assert.Single(response.Data);
        Assert.Equal("37c22147-4194-11ec-8061-0242ac120002", response.Data[0].Id);
    }

    [Fact]
    public void RechargeStatus_AllFiveValuesDeserialize()
    {
        // Schema: PENDING, CONFIRMED, CANCELLED, REFUNDED, WAITING_CRITICAL_ACTION
        foreach (var status in new[] { "PENDING", "CONFIRMED", "CANCELLED", "REFUNDED", "WAITING_CRITICAL_ACTION" })
        {
            var json = $"{{\"id\":\"x\",\"status\":\"{status}\"}}";
            var result = JsonContractAssert.DeserializeFixture<MobilePhoneRecharge>(json);
            Assert.Equal(status, result.Status.ToString());
        }
    }

    // ─────────────────────────────────────────────────────────────
    // GET /v3/mobilePhoneRecharges/{phoneNumber}/provider
    // B-19 regression: MobilePhoneProvider tinha AvailableValues: List<decimal>.
    // Schema real: values: array de { name, description, bonus, minValue, maxValue }.
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void ProviderResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("MobilePhoneRecharge/provider-response.json");

        var result = JsonContractAssert.DeserializeFixture<MobilePhoneProvider>(json);

        Assert.Equal("Vivo", result.Name);
        Assert.Equal(2, result.Values.Count);

        Assert.Equal("R$ 12,00", result.Values[0].Name);
        Assert.Equal("5.0", result.Values[0].Bonus);
        Assert.Equal(1m, result.Values[0].MinValue);
        Assert.Equal(5m, result.Values[0].MaxValue);

        Assert.Equal("R$ 30,00", result.Values[1].Name);
        Assert.Equal("3.0", result.Values[1].Bonus);
        Assert.Equal(20m, result.Values[1].MinValue);
        Assert.Equal(50m, result.Values[1].MaxValue);
    }

    [Fact]
    public void ProviderResponse_UsesValuesNotAvailableValues()
    {
        // Regressao B-19: garantir que o nome JSON e "values" (do schema),
        // nao "availableValues" (que era invencao do modelo antigo).
        var officialJson = "{\"name\":\"Vivo\",\"values\":[{\"name\":\"R$ 12,00\",\"bonus\":\"5.0\",\"minValue\":1,\"maxValue\":5}]}";

        var result = JsonContractAssert.DeserializeFixture<MobilePhoneProvider>(officialJson);

        Assert.Single(result.Values);
        Assert.Equal("R$ 12,00", result.Values[0].Name);
    }
}
