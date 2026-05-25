using System;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.PaymentLink;
using Codout.Apis.Asaas.Models.PaymentLink.Enums;
using Codout.Apis.Asaas.Models.Subscription.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para PaymentLinkManager (B-36).
/// Schemas verificados via MCP em 2026-05-24.
/// </summary>
public class PaymentLinkContractTests
{
    [Fact]
    public void PaymentLinkResponse_DeserializesAllFields()
    {
        var json = "{\"id\":\"725104409743\",\"name\":\"Book sales\",\"value\":50,\"active\":true,\"chargeType\":\"DETACHED\",\"url\":\"https://www.asaas.com/c/291089675759\",\"billingType\":\"UNDEFINED\",\"subscriptionCycle\":\"MONTHLY\",\"description\":\"Any book for just R$: 50.00\",\"endDate\":\"2024-09-05\",\"deleted\":false,\"viewCount\":0,\"maxInstallmentCount\":1,\"dueDateLimitDays\":10,\"notificationEnabled\":true,\"isAddressRequired\":true,\"externalReference\":\"056984\"}";

        var result = JsonContractAssert.DeserializeFixture<PaymentLink>(json);

        Assert.Equal("725104409743", result.Id);
        Assert.Equal("Book sales", result.Name);
        Assert.Equal(50m, result.Value);
        Assert.True(result.Active);
        Assert.Equal(ChargeType.DETACHED, result.ChargeType);
        Assert.Equal("https://www.asaas.com/c/291089675759", result.Url);
        Assert.Equal(BillingType.UNDEFINED, result.BillingType);
        Assert.Equal(Cycle.MONTHLY, result.SubscriptionCycle);
        Assert.Equal(new DateTime(2024, 9, 5), result.EndDate);
        Assert.False(result.Deleted);
        Assert.Equal(0, result.ViewCount);
        Assert.Equal(1, result.MaxInstallmentCount);
        Assert.Equal(10, result.DueDateLimitDays);
        Assert.True(result.NotificationEnabled);
        Assert.True(result.IsAddressRequired);
        Assert.Equal("056984", result.ExternalReference);
    }

    [Fact]
    public void ChargeType_AllThreeValuesDeserialize()
    {
        foreach (var ct in new[] { "DETACHED", "RECURRENT", "INSTALLMENT" })
        {
            var json = $"{{\"id\":\"x\",\"chargeType\":\"{ct}\"}}";
            var result = JsonContractAssert.DeserializeFixture<PaymentLink>(json);
            Assert.Equal(ct, result.ChargeType.ToString());
        }
    }

    [Fact]
    public void SubscriptionCycle_AllSevenValuesDeserialize()
    {
        // B-36a: SubscriptionCycle era string. Schema e Cycle enum.
        foreach (var cycle in new[] {
            "WEEKLY", "BIWEEKLY", "MONTHLY", "BIMONTHLY",
            "QUARTERLY", "SEMIANNUALLY", "YEARLY" })
        {
            var json = $"{{\"id\":\"x\",\"subscriptionCycle\":\"{cycle}\"}}";
            var result = JsonContractAssert.DeserializeFixture<PaymentLink>(json);
            Assert.NotNull(result.SubscriptionCycle);
            Assert.Equal(cycle, result.SubscriptionCycle.ToString());
        }
    }
}
