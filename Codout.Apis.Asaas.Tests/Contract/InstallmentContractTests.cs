using System;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Installment;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para InstallmentManager (B-31).
/// Schemas verificados via MCP em 2026-05-24.
/// </summary>
public class InstallmentContractTests
{
    [Fact]
    public void InstallmentResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("Installment/installment-response.json");

        var result = JsonContractAssert.DeserializeFixture<Installment>(json);

        Assert.Equal("installment", result.Object);
        Assert.Equal("2765d086-c7c5-5cca-898a-4262d212587c", result.Id);
        Assert.Equal(360m, result.Value);
        Assert.Equal(312.12m, result.NetValue);
        Assert.Equal(30m, result.PaymentValue);
        Assert.Equal(12, result.InstallmentCount);
        Assert.Equal(BillingType.CREDIT_CARD, result.BillingType);
        Assert.Equal(31, result.ExpirationDay);
        Assert.Equal(new DateTime(2021, 1, 19), result.DateCreated);
        Assert.Equal("cus_000000001645", result.CustomerId);
        Assert.Equal("997152082166122", result.PaymentLink);
        Assert.False(result.Deleted);
        Assert.NotNull(result.Refunds);
    }

    [Fact]
    public void InstallmentResponse_NullableExpirationDayHandlesMissing()
    {
        var json = "{\"id\":\"i_x\",\"object\":\"installment\"}";

        var result = JsonContractAssert.DeserializeFixture<Installment>(json);

        Assert.Null(result.ExpirationDay);
    }
}
