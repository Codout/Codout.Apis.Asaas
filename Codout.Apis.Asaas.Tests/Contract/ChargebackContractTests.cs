using System;
using Codout.Apis.Asaas.Models.Chargeback;
using Codout.Apis.Asaas.Models.Chargeback.Enums;
using Codout.Apis.Asaas.Models.Common.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para ChargebackManager (B-41).
/// Schemas verificados via MCP em 2026-05-24.
/// </summary>
public class ChargebackContractTests
{
    [Fact]
    public void ChargebackResponse_DeserializesAllFields()
    {
        var json = "{\"id\":\"8e784c3e-afe8-4844-bb93-6b445763\",\"payment\":\"pay_pBtDdshgBD2Rt\",\"installment\":\"b8dd74c-d078-40a0-9ae1-61a66c61a204\",\"customerAccount\":\"cus_000000004085\",\"status\":\"DONE\",\"reason\":\"COMMERCIAL_DISAGREEMENT\",\"disputeStartDate\":\"2024-11-10\",\"value\":2323.45,\"paymentDate\":\"2024-03-10\",\"creditCard\":{\"number\":\"8829\",\"brand\":\"VISA\"},\"disputeStatus\":\"ACCEPTED\",\"deadlineToSendDisputeDocuments\":\"2024-12-10\"}";

        var result = JsonContractAssert.DeserializeFixture<Chargeback>(json);

        Assert.Equal("8e784c3e-afe8-4844-bb93-6b445763", result.Id);
        Assert.Equal("pay_pBtDdshgBD2Rt", result.PaymentId);
        Assert.Equal("b8dd74c-d078-40a0-9ae1-61a66c61a204", result.InstallmentId);
        Assert.Equal("cus_000000004085", result.CustomerAccountId);
        Assert.Equal(ChargebackStatus.DONE, result.Status);
        Assert.Equal(ChargebackReason.COMMERCIAL_DISAGREEMENT, result.Reason);
        Assert.Equal(new DateTime(2024, 11, 10), result.DisputeStartDate);
        Assert.Equal(2323.45m, result.Value);
        Assert.Equal(new DateTime(2024, 3, 10), result.PaymentDate);
        Assert.Equal(ChargebackDisputeStatus.ACCEPTED, result.DisputeStatus);
        Assert.Equal(new DateTime(2024, 12, 10), result.DeadlineToSendDisputeDocuments);

        // B-41a: CreditCard nested objeto novo
        Assert.NotNull(result.CreditCard);
        Assert.Equal("8829", result.CreditCard.Number);
        Assert.Equal(CreditCardBrand.VISA, result.CreditCard.Brand);
    }

    [Fact]
    public void ChargebackStatus_AllFiveValuesDeserialize()
    {
        foreach (var status in new[] { "REQUESTED", "IN_DISPUTE", "DISPUTE_LOST", "REVERSED", "DONE" })
        {
            var json = $"{{\"id\":\"x\",\"status\":\"{status}\"}}";
            var result = JsonContractAssert.DeserializeFixture<Chargeback>(json);
            Assert.Equal(status, result.Status.ToString());
        }
    }

    [Fact]
    public void ChargebackDisputeStatus_AllThreeValuesDeserialize()
    {
        foreach (var ds in new[] { "REQUESTED", "ACCEPTED", "REJECTED" })
        {
            var json = $"{{\"id\":\"x\",\"disputeStatus\":\"{ds}\"}}";
            var result = JsonContractAssert.DeserializeFixture<Chargeback>(json);
            Assert.NotNull(result.DisputeStatus);
            Assert.Equal(ds, result.DisputeStatus.ToString());
        }
    }
}
