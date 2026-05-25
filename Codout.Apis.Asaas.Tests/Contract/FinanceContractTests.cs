using System;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Finance;
using Codout.Apis.Asaas.Models.Payment.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para FinanceManager (B-37).
/// Schemas verificados via MCP em 2026-05-24.
/// </summary>
public class FinanceContractTests
{
    [Fact]
    public void BalanceResponse_DeserializesValue()
    {
        var json = "{\"balance\":5210.96}";
        var result = JsonContractAssert.DeserializeFixture<Balance>(json);
        Assert.Equal(5210.96m, result.Value);
    }

    [Fact]
    public void PaymentStatistics_DeserializesAllFields()
    {
        var json = "{\"quantity\":23,\"value\":9270.4,\"netValue\":9121.54}";
        var result = JsonContractAssert.DeserializeFixture<PaymentStatistics>(json);
        Assert.Equal(23, result.Quantity);
        Assert.Equal(9270.4m, result.Value);
        Assert.Equal(9121.54m, result.NetValue);
    }

    [Fact]
    public void SplitStatistics_DeserializesIncomeAndValue()
    {
        // B-37a: schema usa {income, value}, nao {totalPendingValue, totalReceivedValue}
        var json = "{\"income\":5210.96,\"value\":9270.4}";

        var result = JsonContractAssert.DeserializeFixture<SplitStatistics>(json);

        Assert.Equal(5210.96m, result.Income);
        Assert.Equal(9270.4m, result.Value);
    }

    [Fact]
    public void PaymentStatisticsFilter_SerializesAllElevenFields()
    {
        // B-37b: GetPaymentStatistics nao aceitava filtros. Schema oficial expoe 11.
        var filter = new PaymentStatisticsFilter
        {
            CustomerId = "cus_x",
            BillingType = BillingType.BOLETO,
            Status = PaymentStatus.RECEIVED,
            Anticipated = false,
            DateCreatedGE = new DateTime(2023, 1, 1),
            DateCreatedLE = new DateTime(2023, 12, 31),
            DueDateGE = new DateTime(2023, 2, 1),
            DueDateLE = new DateTime(2023, 11, 30),
            EstimatedCreditDateGE = new DateTime(2023, 3, 1),
            EstimatedCreditDateLE = new DateTime(2023, 10, 31),
            ExternalReference = "ref_42"
        };

        JsonContractAssert.QueryParamEquals(filter, "customer", "cus_x");
        JsonContractAssert.QueryParamEquals(filter, "billingType", "BOLETO");
        JsonContractAssert.QueryParamEquals(filter, "status", "RECEIVED");
        JsonContractAssert.QueryParamEquals(filter, "anticipated", "false");
        JsonContractAssert.QueryParamEquals(filter, "dateCreated[ge]", "2023-01-01");
        JsonContractAssert.QueryParamEquals(filter, "dateCreated[le]", "2023-12-31");
        JsonContractAssert.QueryParamEquals(filter, "dueDate[ge]", "2023-02-01");
        JsonContractAssert.QueryParamEquals(filter, "dueDate[le]", "2023-11-30");
        JsonContractAssert.QueryParamEquals(filter, "estimatedCreditDate[ge]", "2023-03-01");
        JsonContractAssert.QueryParamEquals(filter, "estimatedCreditDate[le]", "2023-10-31");
        JsonContractAssert.QueryParamEquals(filter, "externalReference", "ref_42");
    }
}
