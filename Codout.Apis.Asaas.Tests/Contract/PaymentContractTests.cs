using System;
using System.Text.Json;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Invoice.Enums;
using Codout.Apis.Asaas.Models.Payment;
using Codout.Apis.Asaas.Models.Payment.Enums;

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

    // ─────────────────────────────────────────────────────────────
    // B-26: Payment response + List filter audit (fase 8)
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void PaymentResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("Payment/payment-response.json");

        var result = JsonContractAssert.DeserializeFixture<Payment>(json);

        Assert.Equal("payment", result.Object);
        Assert.Equal("pay_080225913252", result.Id);
        Assert.Equal(new DateTime(2017, 3, 10), result.DateCreated);
        Assert.Equal("cus_G7Dvo4iphUNk", result.CustomerId);
        Assert.Equal(129.9m, result.Value);
        Assert.Equal(124.9m, result.NetValue);
        Assert.Equal(BillingType.BOLETO, result.BillingType);
        Assert.Equal(PaymentStatus.PENDING, result.Status);
        Assert.Equal(new DateTime(2017, 6, 10), result.DueDate);
        Assert.Equal(new DateTime(2017, 6, 10), result.OriginalDueDate);
        Assert.Equal("https://www.asaas.com/i/080225913252", result.InvoiceUrl);
        Assert.Equal("6453", result.NossoNumero);
        Assert.False(result.Deleted);
    }

    [Fact]
    public void PaymentResponse_NullableDatesHandleMissing()
    {
        // B-26a/b/c regression: DateCreated/DueDate/OriginalDueDate eram non-nullable.
        var json = "{\"id\":\"pay_x\",\"status\":\"PENDING\"}";

        var result = JsonContractAssert.DeserializeFixture<Payment>(json);

        Assert.Null(result.DateCreated);
        Assert.Null(result.DueDate);
        Assert.Null(result.OriginalDueDate);
    }

    [Fact]
    public void PaymentsList_UsesStandardEnvelopeWithPagination()
    {
        var json = FixtureLoader.Load("Payment/payments-list-response.json");
        var response = new ResponseList<Payment>(System.Net.HttpStatusCode.OK, json);

        Assert.True(response.WasSuccessful());
        Assert.Equal(1, response.TotalCount);
        Assert.False(response.HasMore);
        Assert.Single(response.Data);
    }

    [Fact]
    public void BillingType_AllSevenValuesDeserialize()
    {
        foreach (var bt in new[] {
            "UNDEFINED", "BOLETO", "CREDIT_CARD", "DEBIT_CARD",
            "TRANSFER", "DEPOSIT", "PIX" })
        {
            var json = $"{{\"id\":\"x\",\"billingType\":\"{bt}\"}}";
            var result = JsonContractAssert.DeserializeFixture<Payment>(json);
            Assert.Equal(bt, result.BillingType.ToString());
        }
    }

    [Fact]
    public void PaymentStatus_AllFourteenValuesDeserialize()
    {
        var values = new[] {
            "PENDING", "RECEIVED", "CONFIRMED", "OVERDUE", "REFUNDED",
            "RECEIVED_IN_CASH", "REFUND_REQUESTED", "REFUND_IN_PROGRESS",
            "CHARGEBACK_REQUESTED", "CHARGEBACK_DISPUTE", "AWAITING_CHARGEBACK_REVERSAL",
            "DUNNING_REQUESTED", "DUNNING_RECEIVED", "AWAITING_RISK_ANALYSIS"
        };
        foreach (var status in values)
        {
            var json = $"{{\"id\":\"x\",\"status\":\"{status}\"}}";
            var result = JsonContractAssert.DeserializeFixture<Payment>(json);
            Assert.Equal(status, result.Status.ToString());
        }
    }

    [Fact]
    public void ListFilter_NewFieldsSerialize()
    {
        // B-26d: customerGroupName, invoiceStatus, estimatedCreditDate,
        // pixQrCodeId, anticipable, user, checkoutSession - estavam faltando.
        var filter = new PaymentListFilter
        {
            CustomerGroupName = "vip",
            InvoiceStatus = InvoiceStatus.AUTHORIZED,
            EstimatedCreditDate = new DateTime(2024, 1, 1),
            PixQrCodeId = "qr_42",
            Anticipable = true,
            User = "operator@example.com",
            CheckoutSession = "co_42"
        };

        JsonContractAssert.QueryParamEquals(filter, "customerGroupName", "vip");
        JsonContractAssert.QueryParamEquals(filter, "invoiceStatus", "AUTHORIZED");
        JsonContractAssert.QueryParamEquals(filter, "estimatedCreditDate", "2024-01-01");
        JsonContractAssert.QueryParamEquals(filter, "pixQrCodeId", "qr_42");
        JsonContractAssert.QueryParamEquals(filter, "anticipable", "true");
        JsonContractAssert.QueryParamEquals(filter, "user", "operator@example.com");
        JsonContractAssert.QueryParamEquals(filter, "checkoutSession", "co_42");
    }

    [Fact]
    public void ListFilter_AllDateRangeFiltersUseLowercaseGeLe()
    {
        // Schema oficial Payment usa [ge]/[le] LOWERCASE (diferente de Invoice
        // que usa [Ge]/[Le] uppercase). Este teste congela esse padrao.
        var filter = new PaymentListFilter
        {
            DateCreatedGE = new DateTime(2024, 1, 1),
            DateCreatedLE = new DateTime(2024, 12, 31),
            PaymentDateGE = new DateTime(2024, 2, 1),
            PaymentDateLE = new DateTime(2024, 11, 30),
            EstimatedCreditDateGE = new DateTime(2024, 3, 1),
            EstimatedCreditDateLE = new DateTime(2024, 10, 31),
            DueDateGE = new DateTime(2024, 4, 1),
            DueDateLE = new DateTime(2024, 9, 30)
        };

        JsonContractAssert.QueryParamEquals(filter, "dateCreated[ge]", "2024-01-01");
        JsonContractAssert.QueryParamEquals(filter, "dateCreated[le]", "2024-12-31");
        JsonContractAssert.QueryParamEquals(filter, "paymentDate[ge]", "2024-02-01");
        JsonContractAssert.QueryParamEquals(filter, "paymentDate[le]", "2024-11-30");
        JsonContractAssert.QueryParamEquals(filter, "estimatedCreditDate[ge]", "2024-03-01");
        JsonContractAssert.QueryParamEquals(filter, "estimatedCreditDate[le]", "2024-10-31");
        JsonContractAssert.QueryParamEquals(filter, "dueDate[ge]", "2024-04-01");
        JsonContractAssert.QueryParamEquals(filter, "dueDate[le]", "2024-09-30");
    }
}
