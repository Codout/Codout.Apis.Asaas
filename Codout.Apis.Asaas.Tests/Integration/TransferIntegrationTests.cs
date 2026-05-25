using System;
using Codout.Apis.Asaas.Models.Transfer;

namespace Codout.Apis.Asaas.Tests.Integration;

public class TransferIntegrationTests : IntegrationTestBase
{
    [IntegrationFact]
    public async Task ListTransfers_WithDateRangeFilter_SerializesCorrectly()
    {
        // B-29h regression sandbox: filter usa [ge]/[le] lowercase.
        var filter = new TransferListFilter
        {
            DateCreatedGE = DateTime.UtcNow.Date.AddDays(-30),
            DateCreatedLE = DateTime.UtcNow.Date
        };

        var result = await Asaas.Transfer.List(0, 5, filter);
        Assert.True(result.WasSuccessful(), $"ListTransfers falhou: {string.Join(",", result.Errors)}");
    }
}
