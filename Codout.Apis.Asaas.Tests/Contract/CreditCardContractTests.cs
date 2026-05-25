using Codout.Apis.Asaas.Models.Common;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.CreditCard;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para CreditCardManager (B-35).
/// Schemas verificados via MCP em 2026-05-24.
/// </summary>
public class CreditCardContractTests
{
    [Fact]
    public void TokenizeResponse_DeserializesFromSchemaShape()
    {
        var json = "{\"creditCardNumber\":\"8829\",\"creditCardBrand\":\"VISA\",\"creditCardToken\":\"a75a1d98-c52d-4a6b-a413-71e00b193c99\"}";

        var result = JsonContractAssert.DeserializeFixture<CreditCard>(json);

        Assert.Equal("8829", result.Number);
        Assert.Equal(CreditCardBrand.VISA, result.Brand);
        Assert.Equal("a75a1d98-c52d-4a6b-a413-71e00b193c99", result.Token);
    }

    [Fact]
    public void CreditCardBrand_AllThirteenValuesDeserialize()
    {
        foreach (var brand in new[] {
            "VISA", "MASTERCARD", "ELO", "DINERS", "DISCOVER", "AMEX",
            "CABAL", "BANESCARD", "CREDZ", "SOROCRED", "CREDSYSTEM", "JCB", "UNKNOWN" })
        {
            var json = $"{{\"creditCardBrand\":\"{brand}\"}}";
            var result = JsonContractAssert.DeserializeFixture<CreditCard>(json);
            Assert.NotNull(result.Brand);
            Assert.Equal(brand, result.Brand.ToString());
        }
    }

    [Fact]
    public void PreAuthorizationConfig_DeserializesDaysToExpire()
    {
        // B-35b regression: model antigo tinha Enabled + AutomaticCaptureDelay
        // (inventados). Schema real: apenas daysToExpire (required).
        var json = "{\"daysToExpire\":5}";

        var result = JsonContractAssert.DeserializeFixture<PreAuthorizationConfig>(json);

        Assert.Equal(5, result.DaysToExpire);
    }

    [Fact]
    public void SavePreAuthorizationConfigRequest_OnlyDaysToExpire()
    {
        var request = new SavePreAuthorizationConfigRequest { DaysToExpire = 25 };
        JsonContractAssert.SerializesWithKeys(request, "daysToExpire");
        // B-35b regression: nao deve serializar enabled nem automaticCaptureDelay
        JsonContractAssert.DoesNotSerializeKey(request, "enabled");
        JsonContractAssert.DoesNotSerializeKey(request, "automaticCaptureDelay");
    }

    [Fact]
    public void TokenizeRequest_HasRequiredKeys()
    {
        var request = new TokenizeCreditCardRequest
        {
            Customer = "cus_x",
            RemoteIp = "1.2.3.4",
            CreditCard = new CreditCardRequest
            {
                HolderName = "John Doe", Number = "1234567890123456",
                ExpiryMonth = "5", ExpiryYear = "2026", Ccv = "123"
            },
            CreditCardHolderInfo = new CreditCardHolderInfoRequest
            {
                Name = "John Doe", Email = "j@example.com", CpfCnpj = "12345678901",
                PostalCode = "01310000", AddressNumber = "150", Phone = "11999998888"
            }
        };

        JsonContractAssert.SerializesWithKeys(request,
            "customer", "creditCard", "creditCardHolderInfo", "remoteIp");
    }
}
