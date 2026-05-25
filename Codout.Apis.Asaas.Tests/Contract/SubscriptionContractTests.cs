using System;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Subscription;
using Codout.Apis.Asaas.Models.Subscription.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para SubscriptionManager (B-27).
/// Schemas verificados via MCP em 2026-05-24.
/// </summary>
public class SubscriptionContractTests
{
    [Fact]
    public void SubscriptionResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("Subscription/subscription-response.json");

        var result = JsonContractAssert.DeserializeFixture<Subscription>(json);

        Assert.Equal("subscription", result.Object);
        Assert.Equal("sub_VXJBYgP2u0eO", result.Id);
        Assert.Equal(new DateTime(2017, 3, 17), result.DateCreated);
        Assert.Equal("cus_0T1mdomVMi39", result.CustomerId);
        Assert.Equal(BillingType.BOLETO, result.BillingType);
        Assert.Equal(Cycle.MONTHLY, result.Cycle);
        Assert.Equal(19.9m, result.Value);
        Assert.Equal(new DateTime(2017, 6, 15), result.NextDueDate);
        Assert.Equal(new DateTime(2018, 6, 15), result.EndDate);
        Assert.Equal(SubscriptionStatus.ACTIVE, result.Status);
        Assert.False(result.Deleted);
        Assert.Equal(12, result.MaxPayments);
        Assert.Equal("356eb0c4-9eb7-4b7f-b2be-d9479af1d29f", result.CheckoutSession);
    }

    [Fact]
    public void SubscriptionResponse_NullableDatesHandleMissing()
    {
        // B-27b/c: DateCreated e NextDueDate eram non-nullable.
        var json = "{\"id\":\"sub_x\",\"status\":\"INACTIVE\"}";

        var result = JsonContractAssert.DeserializeFixture<Subscription>(json);

        Assert.Null(result.DateCreated);
        Assert.Null(result.NextDueDate);
    }

    [Fact]
    public void SubscriptionStatus_AllThreeValuesDeserialize()
    {
        // B-27a: INACTIVE estava faltando no enum (so tinha ACTIVE+EXPIRED).
        foreach (var status in new[] { "ACTIVE", "EXPIRED", "INACTIVE" })
        {
            var json = $"{{\"id\":\"x\",\"status\":\"{status}\"}}";
            var result = JsonContractAssert.DeserializeFixture<Subscription>(json);
            Assert.Equal(status, result.Status.ToString());
        }
    }

    [Fact]
    public void Cycle_AllSevenValuesDeserialize()
    {
        foreach (var cycle in new[] {
            "WEEKLY", "BIWEEKLY", "MONTHLY", "BIMONTHLY",
            "QUARTERLY", "SEMIANNUALLY", "YEARLY" })
        {
            var json = $"{{\"id\":\"x\",\"cycle\":\"{cycle}\"}}";
            var result = JsonContractAssert.DeserializeFixture<Subscription>(json);
            Assert.Equal(cycle, result.Cycle.ToString());
        }
    }

    [Fact]
    public void ListFilter_AllNewFieldsSerialize()
    {
        // B-27e: customerGroupName, status enum, deletedOnly,
        // externalReference, order, sort estavam faltando.
        var filter = new SubscriptionListFilter
        {
            CustomerId = "cus_1",
            CustomerGroupName = "vip",
            BillingType = BillingType.PIX,
            Status = SubscriptionStatus.ACTIVE,
            IncludeDeleted = false,
            DeletedOnly = true,
            ExternalReference = "ext_42",
            Order = "asc",
            Sort = "dateCreated"
        };

        JsonContractAssert.QueryParamEquals(filter, "customer", "cus_1");
        JsonContractAssert.QueryParamEquals(filter, "customerGroupName", "vip");
        JsonContractAssert.QueryParamEquals(filter, "billingType", "PIX");
        JsonContractAssert.QueryParamEquals(filter, "status", "ACTIVE");
        JsonContractAssert.QueryParamEquals(filter, "includeDeleted", "false");
        JsonContractAssert.QueryParamEquals(filter, "deletedOnly", "true");
        JsonContractAssert.QueryParamEquals(filter, "externalReference", "ext_42");
        JsonContractAssert.QueryParamEquals(filter, "order", "asc");
        JsonContractAssert.QueryParamEquals(filter, "sort", "dateCreated");
    }
}
