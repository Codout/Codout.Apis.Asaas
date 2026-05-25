using System;
using Codout.Apis.Asaas.Models.AsaasAccount;
using Codout.Apis.Asaas.Models.Common.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para AsaasAccountManager (B-39).
/// Schemas verificados via MCP em 2026-05-24.
/// </summary>
public class AsaasAccountContractTests
{
    [Fact]
    public void AccountResponse_DeserializesAllFields()
    {
        var json = "{\"object\":\"account\",\"id\":\"4f468235-cec3-482f-b3d0-348af4c7194\",\"name\":\"John Doe\",\"email\":\"john.doe@asaas.com.br\",\"loginEmail\":\"john.doe@asaas.com.br\",\"phone\":null,\"mobilePhone\":null,\"address\":\"Rua Fernando Orlandi\",\"addressNumber\":\"544\",\"province\":\"Jardim Pedra Branca\",\"postalCode\":\"14079-452\",\"cpfCnpj\":\"35381637000150\",\"birthDate\":\"1995-04-12\",\"personType\":\"JURIDICA\",\"companyType\":\"MEI\",\"city\":15478,\"state\":\"SP\",\"country\":\"Brasil\",\"tradingName\":null,\"site\":\"https://www.example.com\",\"walletId\":\"c0c1688f-636b-42c0-b6ee-7339182276b7\",\"accountNumber\":{\"agency\":\"0001\",\"account\":\"3514\",\"accountDigit\":\"3\"},\"commercialInfoExpiration\":{\"isExpired\":false,\"scheduledDate\":\"2025-05-05 00:00:00\"}}";

        var result = JsonContractAssert.DeserializeFixture<Account>(json);

        Assert.Equal("account", result.Object);
        Assert.Equal("4f468235-cec3-482f-b3d0-348af4c7194", result.Id);
        Assert.Equal("John Doe", result.Name);
        Assert.Equal(PersonType.JURIDICA, result.PersonType);
        Assert.Equal(CompanyType.MEI, result.CompanyType);
        // B-39a: City era string, agora int
        Assert.Equal(15478L, result.City);
        Assert.Equal("SP", result.State);
        Assert.Equal(new DateTime(1995, 4, 12), result.BirthDate);
        Assert.Equal("c0c1688f-636b-42c0-b6ee-7339182276b7", result.WalletId);
        // B-39b: campos novos
        Assert.NotNull(result.AccountNumber);
        Assert.Equal("0001", result.AccountNumber.Agency);
        Assert.NotNull(result.CommercialInfoExpiration);
        Assert.False(result.CommercialInfoExpiration.IsExpired);
    }
}
