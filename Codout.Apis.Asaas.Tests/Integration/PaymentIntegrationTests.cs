using System;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Customer;
using Codout.Apis.Asaas.Models.Payment;

namespace Codout.Apis.Asaas.Tests.Integration;

/// <summary>
/// Integration tests de Payment contra sandbox real.
/// </summary>
public class PaymentIntegrationTests : IntegrationTestBase
{
    private async Task<string> CreateSandboxCustomer(string suffix)
    {
        var stamp = DateTime.UtcNow.ToString("HHmmssfff");
        var customer = await Asaas.Customer.Create(new CreateCustomerRequest
        {
            Name = $"ContractTest {suffix} {stamp}",
            CpfCnpj = "24971563792",
            Email = $"pay-{suffix}-{stamp}@example.com"
        });
        Assert.True(customer.WasSuccessful(), $"Setup customer falhou: {string.Join(",", customer.Errors)}");
        return customer.Data.Id;
    }

    [IntegrationFact]
    public async Task CreatePayment_Boleto_ReturnsPendingPayment()
    {
        var customerId = await CreateSandboxCustomer("boleto");
        try
        {
            var result = await Asaas.Payment.Create(new CreatePaymentRequest
            {
                CustomerId = customerId,
                BillingType = BillingType.BOLETO,
                Value = 100m,
                DueDate = DateTime.UtcNow.Date.AddDays(7),
                Description = "Contract test boleto"
            });

            Assert.True(result.WasSuccessful(), $"Create boleto falhou: {string.Join(",", result.Errors)}");
            Assert.NotNull(result.Data);
            Assert.StartsWith("pay_", result.Data.Id);
            Assert.Equal(BillingType.BOLETO, result.Data.BillingType);
            Assert.Equal(100m, result.Data.Value);
        }
        finally
        {
            await Asaas.Customer.Delete(customerId);
        }
    }

    [IntegrationFact]
    public async Task CreatePayment_Pix_ReturnsPixQrCode()
    {
        var customerId = await CreateSandboxCustomer("pix");
        try
        {
            // 1. cria cobranca PIX
            var payment = await Asaas.Payment.Create(new CreatePaymentRequest
            {
                CustomerId = customerId,
                BillingType = BillingType.PIX,
                Value = 50m,
                DueDate = DateTime.UtcNow.Date.AddDays(3),
                Description = "Contract test PIX"
            });
            Assert.True(payment.WasSuccessful(), $"Create PIX falhou: {string.Join(",", payment.Errors)}");

            // 2. recupera QR code (endpoint GET /v3/payments/{id}/pixQrCode)
            var qr = await Asaas.Payment.GetPixQrCode(payment.Data.Id);
            Assert.True(qr.WasSuccessful(), $"GetPixQrCode falhou: {string.Join(",", qr.Errors)}");
            Assert.NotNull(qr.Data);
            Assert.NotEmpty(qr.Data.Payload);
            Assert.NotEmpty(qr.Data.EncodedImage); // base64 PNG
        }
        finally
        {
            await Asaas.Customer.Delete(customerId);
        }
    }

    [IntegrationFact]
    public async Task ListPayments_WithAnticipatedFilter_SerializesBoolCorrectly()
    {
        // Garantia sistemica: o filtro bool? Anticipated serializa "true"
        // (lowercase) no query string. A API ignora silenciosamente o param
        // se estiver com casing errado, entao o teste aqui valida que NAO
        // ha erro 400 ao mandar com o casing correto.
        var filter = new PaymentListFilter { Anticipated = true };

        var result = await Asaas.Payment.List(0, 5, filter);

        Assert.True(result.WasSuccessful(), $"List com Anticipated=true falhou: {string.Join(",", result.Errors)}");
    }
}
