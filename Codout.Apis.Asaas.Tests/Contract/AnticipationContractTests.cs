using System;
using Codout.Apis.Asaas.Models.Anticipation;
using Codout.Apis.Asaas.Models.Anticipation.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para AnticipationManager (B-30).
/// Schemas verificados via MCP em 2026-05-24.
/// </summary>
public class AnticipationContractTests
{
    [Fact]
    public void AnticipationResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("Anticipation/anticipation-response.json");

        var result = JsonContractAssert.DeserializeFixture<Anticipation>(json);

        Assert.Equal("receivableAnticipation", result.Object);
        Assert.Equal("9e7d8639-350f-45c0-8bc3-d4ddc5f4ebac", result.Id);
        Assert.Equal("pay_626366773834", result.PaymentId);
        Assert.Null(result.InstallmentId);
        Assert.Equal(AnticipationStatus.PENDING, result.Status);
        Assert.Equal(new DateTime(2019, 5, 20), result.AnticipationDate);
        Assert.Equal(new DateTime(2019, 5, 26), result.DueDate);
        Assert.Equal(new DateTime(2019, 5, 14), result.RequestDate);
        Assert.Equal(2.33m, result.Fee);
        Assert.Equal(5, result.AnticipationDays);
        Assert.Equal(73.68m, result.NetValue);
        Assert.Equal(80m, result.TotalValue);
        Assert.Equal(76.01m, result.Value);
        Assert.Null(result.DenialObservation);
    }

    [Fact]
    public void AnticipationResponse_NullableDatesHandleMissing()
    {
        // B-30a: AnticipationDate, DueDate, RequestDate eram non-nullable.
        var json = "{\"id\":\"a_x\",\"status\":\"PENDING\"}";

        var result = JsonContractAssert.DeserializeFixture<Anticipation>(json);

        Assert.Null(result.AnticipationDate);
        Assert.Null(result.DueDate);
        Assert.Null(result.RequestDate);
    }

    [Fact]
    public void AnticipationStatus_AllSevenValuesDeserialize()
    {
        foreach (var status in new[] {
            "PENDING", "DENIED", "CREDITED", "DEBITED", "CANCELLED", "OVERDUE", "SCHEDULED" })
        {
            var json = $"{{\"id\":\"x\",\"status\":\"{status}\"}}";
            var result = JsonContractAssert.DeserializeFixture<Anticipation>(json);
            Assert.Equal(status, result.Status.ToString());
        }
    }
}
