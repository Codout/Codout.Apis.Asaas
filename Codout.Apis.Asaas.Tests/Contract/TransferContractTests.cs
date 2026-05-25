using System;
using Codout.Apis.Asaas.Models.Transfer;
using Codout.Apis.Asaas.Models.Transfer.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para TransferManager (B-29).
/// Schemas verificados via MCP em 2026-05-24.
/// </summary>
public class TransferContractTests
{
    [Fact]
    public void BankAccountTransferResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("Transfer/transfer-response.json");

        var result = JsonContractAssert.DeserializeFixture<BankAccountTransfer>(json);

        Assert.Equal("transfer", result.Object);
        Assert.Equal("777eb7c8-b1a2-4356-8fd8-a1b0644b5282", result.Id);
        Assert.Equal(new DateTime(2019, 5, 2), result.DateCreated);
        Assert.Equal(1000m, result.Value);
        Assert.Equal(1000m, result.NetValue);
        Assert.Equal(BankAccountTransferStatus.PENDING, result.Status);
        Assert.Equal(0m, result.TransferFee);
        Assert.Equal(new DateTime(2019, 5, 2), result.EffectiveDate);
        Assert.True(result.Authorized);
        Assert.Equal(TransferOperationType.TED, result.OperationType);
        Assert.NotNull(result.BankAccount);
        Assert.Equal("Banco do Brasil", result.BankAccount.Bank.Name);
        Assert.Equal("001", result.BankAccount.Bank.Code);
    }

    [Fact]
    public void TransferResponse_NullableDatesAndAuthorizedHandleMissing()
    {
        // B-29b/c: DateCreated era non-nullable, Authorized era bool non-nullable.
        var json = "{\"id\":\"t_x\",\"status\":\"PENDING\"}";

        var result = JsonContractAssert.DeserializeFixture<BankAccountTransfer>(json);

        Assert.Null(result.DateCreated);
        Assert.Null(result.Authorized);
    }

    [Fact]
    public void AsaasAccountTransferStatus_AllFiveValuesDeserialize()
    {
        // B-29d: AsaasAccountTransferStatus tinha apenas PENDING/DONE/CANCELLED.
        // Schema unifica todos os transfers no mesmo enum de 5 valores.
        foreach (var status in new[] {
            "PENDING", "BANK_PROCESSING", "DONE", "CANCELLED", "FAILED" })
        {
            var json = $"{{\"id\":\"x\",\"status\":\"{status}\"}}";
            var result = JsonContractAssert.DeserializeFixture<AsaasAccountTransfer>(json);
            Assert.Equal(status, result.Status.ToString());
        }
    }

    [Fact]
    public void TransferOperationType_AllThreeValuesDeserialize()
    {
        // B-29e: campo novo operationType (PIX/TED/INTERNAL) nao existia no model.
        foreach (var op in new[] { "PIX", "TED", "INTERNAL" })
        {
            var json = $"{{\"id\":\"x\",\"operationType\":\"{op}\"}}";
            var result = JsonContractAssert.DeserializeFixture<BankAccountTransfer>(json);
            Assert.NotNull(result.OperationType);
            Assert.Equal(op, result.OperationType.ToString());
        }
    }

    [Fact]
    public void ListFilter_DateRangeFiltersUseLowercaseGeLe()
    {
        // B-29h: faltavam dateCreated[ge]/[le] e transferDate[ge]/[le].
        var filter = new TransferListFilter
        {
            DateCreatedGE = new DateTime(2024, 1, 1),
            DateCreatedLE = new DateTime(2024, 12, 31),
            TransferDateGE = new DateTime(2024, 2, 1),
            TransferDateLE = new DateTime(2024, 11, 30),
            TransferType = TransferType.BANK_ACCOUNT
        };

        JsonContractAssert.QueryParamEquals(filter, "dateCreated[ge]", "2024-01-01");
        JsonContractAssert.QueryParamEquals(filter, "dateCreated[le]", "2024-12-31");
        JsonContractAssert.QueryParamEquals(filter, "transferDate[ge]", "2024-02-01");
        JsonContractAssert.QueryParamEquals(filter, "transferDate[le]", "2024-11-30");
        JsonContractAssert.QueryParamEquals(filter, "type", "BANK_ACCOUNT");
    }
}
