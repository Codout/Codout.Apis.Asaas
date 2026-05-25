using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Finance;
using Codout.Apis.Asaas.Models.Payment.Enums;

namespace Codout.Apis.Asaas.Tests.Integration;

public class FinanceIntegrationTests : IntegrationTestBase
{
    [IntegrationFact]
    public async Task GetBalance_ReturnsValue()
    {
        var result = await Asaas.Finance.GetBalance();
        Assert.True(result.WasSuccessful(), $"GetBalance falhou: {string.Join(",", result.Errors)}");
        Assert.NotNull(result.Data);
    }

    [IntegrationFact]
    public async Task GetPaymentStatistics_WithFilter_SerializesCorrectly()
    {
        // B-37b regression sandbox: filtros novos enviados ao endpoint.
        var filter = new PaymentStatisticsFilter
        {
            BillingType = BillingType.PIX,
            Status = PaymentStatus.RECEIVED,
            Anticipated = false
        };

        var result = await Asaas.Finance.GetPaymentStatistics(filter);
        Assert.True(result.WasSuccessful(), $"GetPaymentStatistics com filter falhou: {string.Join(",", result.Errors)}");
    }

    [IntegrationFact]
    public async Task GetSplitStatistics_ReturnsIncomeAndValue()
    {
        // B-37a regression sandbox: garantir que income/value shape funcionam.
        var result = await Asaas.Finance.GetSplitStatistics();
        Assert.True(result.WasSuccessful(), $"GetSplitStatistics falhou: {string.Join(",", result.Errors)}");
        Assert.NotNull(result.Data);
    }
}
