namespace Codout.Apis.Asaas.Tests.Integration;

public class AnticipationIntegrationTests : IntegrationTestBase
{
    [IntegrationFact]
    public async Task ListAnticipations_ReturnsEnvelope()
    {
        var result = await Asaas.Anticipation.List(0, 5);
        Assert.True(result.WasSuccessful(), $"ListAnticipations falhou: {string.Join(",", result.Errors)}");
    }

    [IntegrationFact]
    public async Task GetLimits_ReturnsAnticipationLimits()
    {
        var result = await Asaas.Anticipation.GetLimits();
        Assert.True(result.WasSuccessful(), $"GetLimits falhou: {string.Join(",", result.Errors)}");
        Assert.NotNull(result.Data);
    }
}
