using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Wallet;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para WalletManager (B-33).
/// Schemas verificados via MCP em 2026-05-24.
/// Wallet e estrutura minima: {object, id}.
/// </summary>
public class WalletContractTests
{
    [Fact]
    public void WalletList_DeserializesEnvelopeWithWalletItems()
    {
        var json = "{\"object\":\"list\",\"hasMore\":false,\"totalCount\":1,\"limit\":10,\"offset\":0,\"data\":[{\"object\":\"wallet\",\"id\":\"0000c712-0a0b-a0b0-0000-031e7ac51a2\"}]}";
        var response = new ResponseList<Wallet>(System.Net.HttpStatusCode.OK, json);

        Assert.True(response.WasSuccessful());
        Assert.Single(response.Data);
        Assert.Equal("wallet", response.Data[0].Object);
        Assert.Equal("0000c712-0a0b-a0b0-0000-031e7ac51a2", response.Data[0].Id);
    }
}
