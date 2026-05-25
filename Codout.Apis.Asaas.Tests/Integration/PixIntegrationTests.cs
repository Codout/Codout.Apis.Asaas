using Codout.Apis.Asaas.Models.Pix;
using Codout.Apis.Asaas.Models.Pix.Enums;

namespace Codout.Apis.Asaas.Tests.Integration;

public class PixIntegrationTests : IntegrationTestBase
{
    [IntegrationFact]
    public async Task ListAddressKeys_ReturnsEnvelope()
    {
        var result = await Asaas.Pix.ListAddressKeys(0, 5);
        Assert.True(result.WasSuccessful(), $"ListAddressKeys falhou: {string.Join(",", result.Errors)}");
    }

    [IntegrationFact]
    public async Task ListTransactions_WithStatusFilter_SerializesEnumCorrectly()
    {
        // B-28a regression sandbox: garantir que filtro com PixTransactionStatus.AWAITING_REQUEST
        // (novo valor adicionado) nao retorna 400.
        var filter = new PixTransactionListFilter { Status = PixTransactionStatus.AWAITING_REQUEST };
        var result = await Asaas.Pix.ListTransactions(0, 5, filter);
        Assert.True(result.WasSuccessful(), $"ListTransactions falhou: {string.Join(",", result.Errors)}");
    }
}
