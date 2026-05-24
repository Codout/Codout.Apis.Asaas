using System;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Bill;
using Codout.Apis.Asaas.Models.Bill.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para BillPaymentManager.
/// Schemas verificados via MCP em 2026-05-24:
/// - POST /v3/bill (required: identificationField; opcional: scheduleDate,
///   description, discount, interest, fine, dueDate, value, externalReference)
/// - GET /v3/bill (envelope padrao)
/// - POST /v3/bill/simulate (identificationField OU barCode)
/// - GET /v3/bill/{id}
/// - POST /v3/bill/{id}/cancel (body vazio)
/// </summary>
public class BillPaymentContractTests
{
    // ─────────────────────────────────────────────────────────────
    // BillPayment response - B-24a/b regression
    // Antes: faltava interest, fine, paymentDate, externalReference;
    //         failReasons era string em vez de array;
    //         enum BillPaymentStatus faltava 2 valores.
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void BillResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("BillPayment/bill-response.json");

        var result = JsonContractAssert.DeserializeFixture<BillPayment>(json);

        Assert.Equal("f1bce822-6f37-4905-8de8-f1af9f2f4bab", result.Id);
        Assert.Equal(BillPaymentStatus.PENDING, result.Status);
        Assert.Equal(29m, result.Value);
        Assert.Equal(0m, result.Discount);
        // B-24a: campos novos
        Assert.Equal(0m, result.Interest);
        Assert.Equal(0m, result.Fine);
        Assert.Null(result.PaymentDate);
        Assert.Null(result.ExternalReference);
        Assert.Equal("03399.77779 29900.000000 04751.101017 1 81510000002990", result.IdentificationField);
        Assert.Equal(new DateTime(2020, 1, 31), result.DueDate);
        Assert.Equal(new DateTime(2020, 1, 31), result.ScheduleDate);
        Assert.Equal(0m, result.Fee);
        Assert.Equal("Celular 01/12", result.Description);
        Assert.Equal("https://www.asaas.com/comprovantes/00016578", result.TransactionReceiptUrl);
        Assert.False(result.CanBeCancelled);
        // B-24a: failReasons agora e array
        Assert.NotNull(result.FailReasons);
        Assert.Empty(result.FailReasons);
    }

    [Fact]
    public void BillResponse_FailReasonsIsArrayOfStrings()
    {
        // B-24a regression: failReasons era string. Schema e array of string.
        var json = "{\"id\":\"x\",\"status\":\"FAILED\",\"failReasons\":[\"Saldo insuficiente\",\"Banco fora do ar\"]}";

        var result = JsonContractAssert.DeserializeFixture<BillPayment>(json);

        Assert.Equal(2, result.FailReasons.Count);
        Assert.Equal("Saldo insuficiente", result.FailReasons[0]);
        Assert.Equal("Banco fora do ar", result.FailReasons[1]);
    }

    [Fact]
    public void BillsList_UsesStandardEnvelopeWithPagination()
    {
        var json = FixtureLoader.Load("BillPayment/bills-list-response.json");
        var response = new ResponseList<BillPayment>(System.Net.HttpStatusCode.OK, json);

        Assert.True(response.WasSuccessful());
        Assert.Equal(1, response.TotalCount);
        Assert.Single(response.Data);
    }

    [Fact]
    public void BillStatus_AllSevenValuesDeserialize()
    {
        // Schema: PENDING, BANK_PROCESSING, PAID, FAILED, CANCELLED,
        //         REFUNDED, AWAITING_CHECKOUT_RISK_ANALYSIS_REQUEST
        // B-24b: REFUNDED e AWAITING_CHECKOUT_RISK_ANALYSIS_REQUEST eram faltantes
        foreach (var status in new[] {
            "PENDING", "BANK_PROCESSING", "PAID", "FAILED",
            "CANCELLED", "REFUNDED", "AWAITING_CHECKOUT_RISK_ANALYSIS_REQUEST" })
        {
            var json = $"{{\"id\":\"x\",\"status\":\"{status}\"}}";
            var result = JsonContractAssert.DeserializeFixture<BillPayment>(json);
            Assert.Equal(status, result.Status.ToString());
        }
    }

    // ─────────────────────────────────────────────────────────────
    // Create request - B-24d regression
    // Apenas identificationField e required; demais opcionais (nullable).
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void CreateRequest_SerializesAllKeysIncludingNewOnes()
    {
        var request = new CreateBillPaymentRequest
        {
            IdentificationField = "03399.77779 29900.000000 04751.101017 1 81510000002990",
            ScheduleDate = new DateTime(2020, 3, 15),
            Description = "Celular 03/12",
            Discount = 0m,
            Interest = 0m,
            Fine = 0m,
            DueDate = new DateTime(2020, 3, 30),
            Value = 29m,
            ExternalReference = "056984"
        };

        JsonContractAssert.SerializesWithKeys(request,
            "identificationField", "scheduleDate", "description",
            "discount", "interest", "fine", "dueDate", "value", "externalReference");
    }

    [Fact]
    public void CreateRequest_OptionalFieldsOmittedWhenNull()
    {
        // Apenas identificationField e required no schema. Resto pode ser null.
        var request = new CreateBillPaymentRequest
        {
            IdentificationField = "03399.77779 29900.000000 04751.101017 1 81510000002990"
        };

        JsonContractAssert.SerializesWithKeys(request, "identificationField");
        JsonContractAssert.DoesNotSerializeKey(request, "scheduleDate");
        JsonContractAssert.DoesNotSerializeKey(request, "discount");
        JsonContractAssert.DoesNotSerializeKey(request, "value");
    }

    // ─────────────────────────────────────────────────────────────
    // BankSlipInfo response - B-24e regression
    // Schema tem 17 campos (incluindo bank/beneficiary*/min-max/allowChangeValue/
    // discountValue/interestValue/fineValue/originalValue/totalDiscount/
    // totalAdditional/isOverdue). Antes tinha apenas 5 (com bankCode errado).
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void SimulateResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("BillPayment/simulate-response.json");

        var result = JsonContractAssert.DeserializeFixture<SimulatedBillPayment>(json);

        Assert.Equal(new DateTime(2021, 11, 22), result.MinimumScheduleDate);
        Assert.Equal(0m, result.Fee);

        var info = result.BankSlipInfo;
        Assert.NotNull(info);
        Assert.Equal("03399201595100529040147600301023888440000421177", info.IdentificationField);
        Assert.Equal(4211.77m, info.Value);
        Assert.Equal(new DateTime(2021, 12, 24), info.DueDate);
        // B-24e: schema usa "bank", nao "bankCode"
        Assert.Equal("033", info.Bank);
        Assert.Equal("19.540.550/0001-21", info.BeneficiaryCpfCnpj);
        Assert.Equal("ASAAS GESTAO FINANCEIRA S.A.", info.BeneficiaryName);
        Assert.False(info.AllowChangeValue);
        Assert.Equal(4211.77m, info.MinValue);
        Assert.Equal(4211.77m, info.MaxValue);
        Assert.Equal(0m, info.DiscountValue);
        Assert.Equal(0m, info.InterestValue);
        Assert.Equal(0m, info.FineValue);
        Assert.Equal(4211.77m, info.OriginalValue);
        Assert.Equal(0m, info.TotalDiscountValue);
        Assert.Equal(0m, info.TotalAdditionalValue);
        Assert.False(info.IsOverdue);
    }

    // ─────────────────────────────────────────────────────────────
    // Simulate request - identificationField OR barCode
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void SimulateRequest_AcceptsIdentificationFieldOrBarCode()
    {
        var byField = new SimulateBillPaymentRequest { IdentificationField = "03399..." };
        var byBarCode = new SimulateBillPaymentRequest { BarCode = "23793..." };

        JsonContractAssert.SerializesWithKeys(byField, "identificationField");
        JsonContractAssert.SerializesWithKeys(byBarCode, "barCode");
    }
}
