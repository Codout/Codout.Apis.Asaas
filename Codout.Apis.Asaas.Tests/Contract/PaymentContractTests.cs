using System.Text.Json;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Payment;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para endpoints novos do PaymentManager auditados na
/// rodada final (/v3/payments/limits e /v3/payments/simulate).
/// Schemas verificados via MCP em 2026-05-24.
/// </summary>
public class PaymentContractTests
{
    // ─────────────────────────────────────────────────────────────
    // GET /v3/payments/limits  -> PaymentLimits
    // Schema: { creation: { daily: { limit, used, wasReached } } }
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void PaymentLimits_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("Payment/limits-response.json");

        var result = JsonContractAssert.DeserializeFixture<PaymentLimits>(json);

        Assert.NotNull(result.Creation);
        Assert.NotNull(result.Creation.Daily);
        Assert.Equal(10, result.Creation.Daily.Limit);
        Assert.Equal(5, result.Creation.Daily.Used);
        Assert.False(result.Creation.Daily.WasReached);
    }

    // ─────────────────────────────────────────────────────────────
    // POST /v3/payments/simulate  -> SimulatedPayment
    // Request: { value, billingTypes: [...], installmentCount? }
    // Response: { value, creditCard?: {...}, bankSlip?: {...}, pix?: {...} }
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void SimulatePaymentRequest_Minimal_HasExactKeys()
    {
        var request = new SimulatePaymentRequest
        {
            Value = 100m,
            BillingTypes = [BillingType.CREDIT_CARD, BillingType.BOLETO, BillingType.PIX]
        };

        JsonContractAssert.SerializesWithKeys(request, "value", "billingTypes");
    }

    [Fact]
    public void SimulatePaymentRequest_BillingTypesIsArray_NotSingular()
    {
        var request = new SimulatePaymentRequest
        {
            Value = 100m,
            BillingTypes = [BillingType.PIX]
        };

        // Regressao B-03: havia campo BillingType singular antes.
        JsonContractAssert.SerializesWithKeys(request, "billingTypes");
        JsonContractAssert.DoesNotSerializeKey(request, "billingType");
        JsonContractAssert.DoesNotSerializeKey(request, "discountValue");
        JsonContractAssert.DoesNotSerializeKey(request, "splits");
    }

    [Fact]
    public void SimulatePaymentRequest_BillingTypesSerializesAsUppercaseEnums()
    {
        var request = new SimulatePaymentRequest
        {
            Value = 100m,
            BillingTypes = [BillingType.CREDIT_CARD, BillingType.BOLETO]
        };

        var json = JsonSerializer.Serialize<object>(request, JsonSerializerConfiguration.Options);
        Assert.Contains("\"CREDIT_CARD\"", json);
        Assert.Contains("\"BOLETO\"", json);
    }

    [Fact]
    public void SimulatedPaymentResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("Payment/simulate-response.json");

        var result = JsonContractAssert.DeserializeFixture<SimulatedPayment>(json);

        Assert.Equal(100m, result.Value);

        Assert.NotNull(result.CreditCard);
        Assert.Equal(100m, result.CreditCard.NetValue);
        Assert.Equal(2.49m, result.CreditCard.FeePercentage);
        Assert.Equal(0.49m, result.CreditCard.OperationFee);
        Assert.Equal(50m, result.CreditCard.Installment.PaymentValue);
        Assert.Equal(48.52m, result.CreditCard.Installment.PaymentNetValue);

        Assert.NotNull(result.BankSlip);
        Assert.Equal(98.02m, result.BankSlip.NetValue);
        Assert.Equal(0.99m, result.BankSlip.FeeValue);

        Assert.NotNull(result.Pix);
        Assert.Equal(98.02m, result.Pix.NetValue);
        Assert.Null(result.Pix.FeePercentage);
        Assert.Equal(0.99m, result.Pix.FeeValue);
    }

    // ─────────────────────────────────────────────────────────────
    // Error envelope: { errors: [{ code, description }] }
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void ErrorResponse_FromOfficialFixture_DeserializesViaResponseObject()
    {
        var json = FixtureLoader.Load("error-response.json");

        var response = new ResponseObject<SimulatedPayment>(
            System.Net.HttpStatusCode.BadRequest, json);

        Assert.False(response.WasSuccessful());
        Assert.Single(response.Errors);
        Assert.Equal("invalid_object", response.Errors[0].Code);
        Assert.Equal("Informe o número de parcelas.", response.Errors[0].Description);
    }
}
