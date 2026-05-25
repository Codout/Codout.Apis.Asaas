using System;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Customer;
using Codout.Apis.Asaas.Models.Subscription;
using Codout.Apis.Asaas.Models.Subscription.Enums;

namespace Codout.Apis.Asaas.Tests.Integration;

public class SubscriptionIntegrationTests : IntegrationTestBase
{
    private async Task<string> CreateSandboxCustomer(string suffix)
    {
        var stamp = DateTime.UtcNow.ToString("HHmmssfff");
        var c = await Asaas.Customer.Create(new CreateCustomerRequest
        {
            Name = $"ContractTest {suffix} {stamp}",
            CpfCnpj = "24971563792",
            Email = $"sub-{suffix}-{stamp}@example.com"
        });
        Assert.True(c.WasSuccessful(), $"Setup customer falhou: {string.Join(",", c.Errors)}");
        return c.Data.Id;
    }

    [IntegrationFact]
    public async Task CreateSubscription_Boleto_RoundTrip()
    {
        var customerId = await CreateSandboxCustomer("sub");
        try
        {
            var created = await Asaas.Subscription.Create(new CreateSubscriptionRequest
            {
                CustomerId = customerId,
                BillingType = BillingType.BOLETO,
                Value = 19.9m,
                NextDueDate = DateTime.UtcNow.Date.AddDays(7),
                Cycle = Cycle.MONTHLY,
                Description = "Contract test subscription"
            });

            Assert.True(created.WasSuccessful(), $"Create sub falhou: {string.Join(",", created.Errors)}");
            Assert.StartsWith("sub_", created.Data.Id);
            Assert.Equal(SubscriptionStatus.ACTIVE, created.Data.Status);

            // Cleanup
            await Asaas.Subscription.Delete(created.Data.Id);
        }
        finally
        {
            await Asaas.Customer.Delete(customerId);
        }
    }

    [IntegrationFact]
    public async Task ListSubscriptions_WithFilterAndStatus_SerializesEnumCorrectly()
    {
        // B-27a regression sandbox: garantir que filtro com SubscriptionStatus.INACTIVE
        // (valor novo do enum) nao retorna 400.
        var filter = new SubscriptionListFilter { Status = SubscriptionStatus.INACTIVE };
        var result = await Asaas.Subscription.List(0, 5, filter);
        Assert.True(result.WasSuccessful(), $"List com status=INACTIVE falhou: {string.Join(",", result.Errors)}");
    }
}
