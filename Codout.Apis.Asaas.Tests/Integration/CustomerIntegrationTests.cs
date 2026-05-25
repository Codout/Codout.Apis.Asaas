using System;
using Codout.Apis.Asaas.Models.Customer;

namespace Codout.Apis.Asaas.Tests.Integration;

/// <summary>
/// Integration tests reais contra api-sandbox.asaas.com.
/// Skip automatico se ASAAS_SANDBOX_TOKEN nao estiver definido.
///
/// Para rodar:
///   $env:ASAAS_SANDBOX_TOKEN = "aact_YTU0...seu_token_sandbox..."
///   dotnet test --filter "Category=Integration"
/// </summary>
public class CustomerIntegrationTests : IntegrationTestBase
{
    [IntegrationFact]
    public async Task Create_Find_Update_Delete_Customer_RoundTrip()
    {
        // Suffix unico por execucao para evitar colisao no sandbox
        var stamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");

        // 1. CREATE
        var created = await Asaas.Customer.Create(new CreateCustomerRequest
        {
            Name = $"ContractTest Customer {stamp}",
            Email = $"contract+{stamp}@example.com",
            CpfCnpj = "24971563792", // CPF sandbox valido (geradores publicos)
            MobilePhone = "11999998888",
            ExternalReference = $"contract-{stamp}"
        });

        Assert.True(created.WasSuccessful(), $"Create falhou: {string.Join(",", created.Errors)}");
        Assert.NotNull(created.Data);
        Assert.StartsWith("cus_", created.Data.Id);
        Assert.Equal($"ContractTest Customer {stamp}", created.Data.Name);

        // 2. FIND
        var found = await Asaas.Customer.Find(created.Data.Id);
        Assert.True(found.WasSuccessful());
        Assert.Equal(created.Data.Id, found.Data.Id);
        Assert.Equal(created.Data.Email, found.Data.Email);

        // 3. UPDATE
        var updated = await Asaas.Customer.Update(created.Data.Id, new UpdateCustomerRequest
        {
            Name = $"ContractTest Customer {stamp} (updated)"
        });
        Assert.True(updated.WasSuccessful());
        Assert.Equal($"ContractTest Customer {stamp} (updated)", updated.Data.Name);

        // 4. DELETE (cleanup)
        var deleted = await Asaas.Customer.Delete(created.Data.Id);
        Assert.True(deleted.WasSuccessful());
    }

    [IntegrationFact]
    public async Task List_RespectsOffsetAndLimit_AndUsesStandardEnvelope()
    {
        var result = await Asaas.Customer.List(0, 3);

        Assert.True(result.WasSuccessful());
        Assert.NotNull(result.Data);
        Assert.True(result.Data.Count <= 3, "limit=3 deveria garantir <=3 items");
        Assert.Equal(3, result.Limit);
        Assert.Equal(0, result.Offset);
        Assert.True(result.TotalCount >= 0);
    }
}
