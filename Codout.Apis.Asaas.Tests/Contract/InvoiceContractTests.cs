using System;
using Codout.Apis.Asaas.Models.Common;
using Codout.Apis.Asaas.Models.Invoice;
using Codout.Apis.Asaas.Models.Invoice.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para InvoiceManager.
/// Schemas verificados via MCP em 2026-05-24:
/// - POST /v3/invoices (request InvoiceSaveRequestDTO, response InvoiceGetResponseDTO)
/// - GET /v3/invoices (envelope padrao + filtros)
/// - PUT /v3/invoices/{id} (InvoiceUpdateRequestDTO)
/// - GET /v3/invoices/{id}
/// - POST /v3/invoices/{id}/authorize (body vazio)
/// - POST /v3/invoices/{id}/cancel (body vazio)
/// </summary>
public class InvoiceContractTests
{
    // ─────────────────────────────────────────────────────────────
    // CreateInvoiceRequest - required: serviceDescription, observations, value,
    // deductions, effectiveDate, municipalServiceName, taxes
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void CreateRequest_SerializesAllExpectedKeys()
    {
        var request = new CreateInvoiceRequest
        {
            PaymentId = "pay_637959110194",
            CustomerId = "cus_000000002750",
            ServiceDescription = "Invoice 101940",
            Observations = "Monthly for June work.",
            Value = 300m,
            Deductions = 10m,
            EffectiveDate = new DateTime(2024, 8, 20),
            MunicipalServiceName = "Systems analysis and development",
            MunicipalServiceCode = "1.01",
            Taxes = new Taxes { RetainIss = true, Iss = 2m, Pis = 0.65m, Cofins = 3m, Csll = 9m, Inss = 11m, Ir = 1.5m }
        };

        JsonContractAssert.SerializesWithKeys(request,
            "payment", "customer", "serviceDescription", "observations",
            "value", "deductions", "effectiveDate",
            "municipalServiceName", "municipalServiceCode", "taxes");
    }

    [Fact]
    public void CreateRequest_UsesPaymentNotPaymentId()
    {
        // Regressao: campo deve serializar como "payment" (do schema), nao "paymentId".
        var request = new CreateInvoiceRequest { PaymentId = "pay_x" };
        JsonContractAssert.SerializesWithKeys(request, "payment");
        JsonContractAssert.DoesNotSerializeKey(request, "paymentId");
        JsonContractAssert.DoesNotSerializeKey(request, "customerId");
        JsonContractAssert.DoesNotSerializeKey(request, "installmentId");
    }

    [Fact]
    public void CreateRequest_SupportsUpdatePaymentFlag()
    {
        // Schema InvoiceSaveRequestDTO inclui updatePayment opcional.
        var request = new CreateInvoiceRequest { Value = 100m, UpdatePayment = true };
        JsonContractAssert.SerializesWithKeys(request, "updatePayment");
    }

    // ─────────────────────────────────────────────────────────────
    // UpdateInvoiceRequest - InvoiceUpdateRequestDTO (todos opcionais)
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void UpdateRequest_SupportsUpdatePaymentFlag()
    {
        var request = new UpdateInvoiceRequest { UpdatePayment = false };
        JsonContractAssert.SerializesWithKeys(request, "updatePayment");
    }

    // ─────────────────────────────────────────────────────────────
    // Invoice response - InvoiceGetResponseDTO + Taxes com Reforma Tributaria
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void InvoiceResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("Invoice/invoice-response.json");

        var result = JsonContractAssert.DeserializeFixture<Invoice>(json);

        Assert.Equal("inv_000000000232", result.Id);
        Assert.Equal(InvoiceStatus.SCHEDULED, result.Status);
        Assert.Equal("cus_000000002750", result.CustomerId);
        Assert.Equal("pay_145059895800", result.PaymentId);
        Assert.Equal("NFS-e", result.Type);
        Assert.Equal(300m, result.Value);
        Assert.Equal(10m, result.Deductions);
        Assert.Equal(new DateTime(2024, 8, 15), result.EffectiveDate);
        Assert.Equal("1.01", result.MunicipalServiceCode);

        Assert.NotNull(result.Taxes);
    }

    [Fact]
    public void TaxesResponse_HasAllReformaTributariaFields()
    {
        // B-21a: schema InvoiceTaxesResponseDTO inclui stateIbs, stateIbsValue,
        // municipalIbs, municipalIbsValue, cbs, cbsValue + nbsCode,
        // taxSituationCode, taxClassificationCode, operationIndicatorCode,
        // pisCofinsRetentionType, pisCofinsTaxStatus. Antes do fix, esses
        // campos sumiam silenciosamente na deserializacao.
        var json = FixtureLoader.Load("Invoice/invoice-response.json");
        var result = JsonContractAssert.DeserializeFixture<Invoice>(json);

        var t = result.Taxes;
        Assert.Equal("1.0101.11.00", t.NbsCode);
        Assert.Equal("011", t.TaxSituationCode);
        Assert.Equal("011001", t.TaxClassificationCode);
        Assert.Equal("020101", t.OperationIndicatorCode);
        Assert.True(t.RetainIss);
        Assert.Equal(2m, t.Iss);
        Assert.Equal("NOT_WITHHELD", t.PisCofinsRetentionType);
        Assert.Equal("STANDARD_TAXABLE_OPERATION", t.PisCofinsTaxStatus);
        Assert.Equal(0.65m, t.Pis);
        Assert.Equal(3m, t.Cofins);
        Assert.Equal(9m, t.Csll);
        Assert.Equal(11m, t.Inss);
        Assert.Equal(1.5m, t.Ir);
        Assert.Equal(0.1m, t.StateIbs);
        Assert.Equal(0.3m, t.StateIbsValue);
        Assert.Equal(0m, t.MunicipalIbs);
        Assert.Equal(0m, t.MunicipalIbsValue);
        Assert.Equal(0.9m, t.Cbs);
        Assert.Equal(2.7m, t.CbsValue);
    }

    [Fact]
    public void InvoiceStatus_AllSixValuesDeserialize()
    {
        foreach (var status in new[] {
            "SCHEDULED", "AUTHORIZED", "PROCESSING_CANCELLATION",
            "CANCELED", "CANCELLATION_DENIED", "ERROR" })
        {
            var json = $"{{\"id\":\"x\",\"status\":\"{status}\"}}";
            var result = JsonContractAssert.DeserializeFixture<Invoice>(json);
            Assert.Equal(status, result.Status.ToString());
        }
    }

    // ─────────────────────────────────────────────────────────────
    // InvoiceListFilter - B-21b/c regression
    // Schema usa effectiveDate[Ge] e [Le] com G/L MAIUSCULOS.
    // Antes: lowercase [ge]/[le] (silenciosamente ignorado pela API).
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void ListFilter_UsesCapitalGeAndLeForEffectiveDate()
    {
        var filter = new InvoiceListFilter
        {
            EffectiveDateGE = new DateTime(2024, 8, 3),
            EffectiveDateLE = new DateTime(2024, 9, 3)
        };

        JsonContractAssert.QueryParamEquals(filter, "effectiveDate[Ge]", "2024-08-03");
        JsonContractAssert.QueryParamEquals(filter, "effectiveDate[Le]", "2024-09-03");

        // Regressao B-21b: confirmar que NAO esta serializando com minuscula
        Assert.False(filter.ContainsKey("effectiveDate[ge]"));
        Assert.False(filter.ContainsKey("effectiveDate[le]"));
    }

    [Fact]
    public void ListFilter_SupportsCustomerAndExternalReference()
    {
        // B-21c: faltavam os filtros customer e externalReference.
        var filter = new InvoiceListFilter
        {
            CustomerId = "cus_000000002750",
            ExternalReference = "ext_ref_42",
            PaymentId = "pay_x",
            InstallmentId = "ins_x",
            Status = InvoiceStatus.AUTHORIZED
        };

        JsonContractAssert.QueryParamEquals(filter, "customer", "cus_000000002750");
        JsonContractAssert.QueryParamEquals(filter, "externalReference", "ext_ref_42");
        JsonContractAssert.QueryParamEquals(filter, "payment", "pay_x");
        JsonContractAssert.QueryParamEquals(filter, "installment", "ins_x");
        JsonContractAssert.QueryParamEquals(filter, "status", "AUTHORIZED");
    }
}
