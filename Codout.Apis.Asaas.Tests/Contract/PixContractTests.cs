using System;
using Codout.Apis.Asaas.Models.Pix;
using Codout.Apis.Asaas.Models.Pix.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para PixManager (B-28).
/// Schemas verificados via MCP em 2026-05-24.
/// </summary>
public class PixContractTests
{
    [Fact]
    public void TransactionResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("Pix/transaction-response.json");

        var result = JsonContractAssert.DeserializeFixture<PixTransaction>(json);

        Assert.Equal("35363f6e-93e2-11ec-b9d9-96f4053b1bd4", result.Id);
        Assert.Equal("E00416968202111161635q5bk0brYk2C", result.EndToEndIdentifier);
        Assert.Equal(PixTransactionFinality.WITHDRAWAL, result.Finality);
        Assert.Equal(10m, result.Value);
        Assert.Equal(0m, result.RefundedValue);
        Assert.Equal(new DateTime(2022, 1, 13, 10, 49, 59), result.EffectiveDate);
        Assert.Equal(new DateTime(2022, 10, 18), result.ScheduledDate);
        Assert.Equal(PixTransactionStatus.DONE, result.Status);
        Assert.Equal(PixTransactionType.DEBIT, result.Type);
        Assert.Equal(PixTransactionOriginType.DYNAMIC_QRCODE, result.OriginType);
        Assert.Equal("dcabae5bbfb6nffbb87c693883656483", result.ConciliationIdentifier);
        Assert.True(result.CanBeCanceled);
        Assert.True(result.CanBeRefunded);
        Assert.Equal(0.99m, result.ChargedFeeValue);
        Assert.Equal("pay_0491859546906926", result.Payment);
        Assert.Equal(new DateTime(2023, 2, 14, 10, 42, 55), result.DateCreated);

        Assert.NotNull(result.ExternalAccount);
        Assert.Equal("416968", result.ExternalAccount.Ispb);
        Assert.Equal("Example Bank S.A", result.ExternalAccount.IspbName);
        Assert.Equal("John Doe", result.ExternalAccount.Name);
        Assert.Equal(PixAddressKeyType.CPF, result.ExternalAccount.AddressKeyType);
    }

    [Fact]
    public void TransactionStatus_AllElevenValuesDeserialize()
    {
        // B-28a regression: enum tinha apenas 5 valores (PENDING, DONE, CANCELLED,
        // SCHEDULED, FAILED). PENDING e FAILED nem existem no schema; faltavam 8
        // valores corretos. Sem o fix, deserializar AWAITING_BALANCE_VALIDATION
        // ou REQUESTED ou REFUSED lancava exception.
        foreach (var status in new[] {
            "AWAITING_BALANCE_VALIDATION", "AWAITING_INSTANT_PAYMENT_ACCOUNT_BALANCE",
            "AWAITING_CRITICAL_ACTION_AUTHORIZATION", "AWAITING_CHECKOUT_RISK_ANALYSIS_REQUEST",
            "AWAITING_CASH_IN_RISK_ANALYSIS_REQUEST", "SCHEDULED", "AWAITING_REQUEST",
            "REQUESTED", "DONE", "REFUSED", "CANCELLED" })
        {
            var json = $"{{\"id\":\"x\",\"status\":\"{status}\"}}";
            var result = JsonContractAssert.DeserializeFixture<PixTransaction>(json);
            Assert.Equal(status, result.Status.ToString());
        }
    }

    [Fact]
    public void TransactionType_AllFiveValuesDeserialize()
    {
        foreach (var type in new[] {
            "DEBIT", "CREDIT", "CREDIT_REFUND", "DEBIT_REFUND", "DEBIT_REFUND_CANCELLATION" })
        {
            var json = $"{{\"id\":\"x\",\"type\":\"{type}\"}}";
            var result = JsonContractAssert.DeserializeFixture<PixTransaction>(json);
            Assert.NotNull(result.Type);
            Assert.Equal(type, result.Type.ToString());
        }
    }

    [Fact]
    public void TransactionOriginType_AllSixValuesDeserialize()
    {
        foreach (var origin in new[] {
            "MANUAL", "ADDRESS_KEY", "STATIC_QRCODE", "DYNAMIC_QRCODE",
            "PAYMENT_INITIATION_SERVICE", "AUTOMATIC_RECURRING" })
        {
            var json = $"{{\"id\":\"x\",\"originType\":\"{origin}\"}}";
            var result = JsonContractAssert.DeserializeFixture<PixTransaction>(json);
            Assert.NotNull(result.OriginType);
            Assert.Equal(origin, result.OriginType.ToString());
        }
    }

    [Fact]
    public void AddressKeyResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("Pix/address-key-response.json");

        var result = JsonContractAssert.DeserializeFixture<PixAddressKey>(json);

        Assert.Equal("a33047b1-fb19-4b68-9373-a7ba8a8162aa", result.Id);
        Assert.Equal("b6295ee1-f054-47d1-9e90-ee57b74f60d9", result.Key);
        Assert.Equal(PixAddressKeyType.EVP, result.Type);
        Assert.Equal(PixAddressKeyStatus.ACTIVE, result.Status);
        Assert.Equal(new DateTime(2022, 2, 7, 17, 17, 48), result.DateCreated);
        Assert.True(result.CanBeDeleted);

        Assert.NotNull(result.QrCode);
        Assert.Equal("QRCODE IMAGE IN BASE64", result.QrCode.EncodedImage);
        Assert.NotEmpty(result.QrCode.Payload);
    }

    [Fact]
    public void AddressKeyStatus_AllSixValuesDeserialize()
    {
        // B-28c: Status era string em vez de enum.
        foreach (var status in new[] {
            "AWAITING_ACTIVATION", "ACTIVE", "AWAITING_DELETION",
            "AWAITING_ACCOUNT_DELETION", "DELETED", "ERROR" })
        {
            var json = $"{{\"id\":\"x\",\"status\":\"{status}\"}}";
            var result = JsonContractAssert.DeserializeFixture<PixAddressKey>(json);
            Assert.Equal(status, result.Status.ToString());
        }
    }

    [Fact]
    public void ListTransactionsFilter_NewFiltersSerialize()
    {
        // B-28e: ListTransactions nao aceitava filtro.
        var filter = new PixTransactionListFilter
        {
            Status = PixTransactionStatus.DONE,
            Type = PixTransactionType.CREDIT,
            EndToEndIdentifier = "E00416968202111161635q5bk0brYk2C"
        };

        JsonContractAssert.QueryParamEquals(filter, "status", "DONE");
        JsonContractAssert.QueryParamEquals(filter, "type", "CREDIT");
        JsonContractAssert.QueryParamEquals(filter, "endToEndIdentifier", "E00416968202111161635q5bk0brYk2C");
    }
}
