using System;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.MyAccount;
using Codout.Apis.Asaas.Models.MyAccount.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para MyAccountManager (resto alem de Documents §7) — B-38.
/// Schemas verificados via MCP em 2026-05-24.
/// </summary>
public class MyAccountContractTests
{
    [Fact]
    public void CommercialInfoResponse_DeserializesAllFields()
    {
        var json = "{\"status\":\"APPROVED\",\"personType\":\"JURIDICA\",\"cpfCnpj\":\"66625514000140\",\"name\":\"John Doe\",\"birthDate\":\"1995-04-12\",\"companyName\":null,\"companyType\":\"MEI\",\"incomeValue\":250000,\"email\":\"john.doe@asaas.com.br\",\"phone\":null,\"mobilePhone\":null,\"postalCode\":\"89223005\",\"address\":\"Av. Rolf Wiest\",\"addressNumber\":\"659\",\"complement\":null,\"province\":\"Bom retiro\",\"city\":null,\"denialReason\":null,\"tradingName\":null,\"site\":null,\"availableCompanyNames\":[\"ASAAS\",\"ASAAS GESTAO FINANCEIRA S.A.\"],\"commercialInfoExpiration\":{\"isExpired\":false,\"scheduledDate\":\"2025-05-05 00:00:00\"}}";

        var result = JsonContractAssert.DeserializeFixture<MyAccount>(json);

        // B-38a: Status era string. Schema e enum AccountInfoStatus.
        Assert.Equal(AccountInfoStatus.APPROVED, result.Status);
        Assert.Equal(PersonType.JURIDICA, result.PersonType);
        Assert.Equal("66625514000140", result.CpfCnpj);
        Assert.Equal("John Doe", result.Name);
        Assert.Equal(new DateTime(1995, 4, 12), result.BirthDate);
        Assert.Equal(CompanyType.MEI, result.CompanyType);
        Assert.Equal(250000m, result.IncomeValue);
        Assert.Equal("89223005", result.PostalCode);
        // B-38b: campos novos
        Assert.Equal(2, result.AvailableCompanyNames.Count);
        Assert.NotNull(result.CommercialInfoExpiration);
        Assert.False(result.CommercialInfoExpiration.IsExpired);
        Assert.Equal(new DateTime(2025, 5, 5), result.CommercialInfoExpiration.ScheduledDate);
    }

    [Fact]
    public void AccountInfoStatus_AllFourValuesDeserialize()
    {
        foreach (var status in new[] { "APPROVED", "AWAITING_ACTION_AUTHORIZATION", "DENIED", "PENDING" })
        {
            var json = $"{{\"status\":\"{status}\"}}";
            var result = JsonContractAssert.DeserializeFixture<MyAccount>(json);
            Assert.NotNull(result.Status);
            Assert.Equal(status, result.Status.ToString());
        }
    }

    [Fact]
    public void AccountStatus_AllFourApprovalStatusValuesDeserialize()
    {
        // AccountStatus (GET /myAccount/status) ja usava AccountApprovalStatus
        // pre-existente — apenas confirmar 4 valores.
        foreach (var status in new[] { "PENDING", "APPROVED", "REJECTED", "AWAITING_APPROVAL" })
        {
            var json = $"{{\"id\":\"x\",\"commercialInfo\":\"{status}\"}}";
            var result = JsonContractAssert.DeserializeFixture<AccountStatus>(json);
            Assert.NotNull(result.CommercialInfo);
            Assert.Equal(status, result.CommercialInfo.ToString());
        }
    }
}
