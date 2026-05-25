using Codout.Apis.Asaas.Models.FiscalInfo;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para FiscalInfoManager (B-40).
/// Schemas verificados via MCP em 2026-05-24.
/// </summary>
public class FiscalInfoContractTests
{
    [Fact]
    public void FiscalInfoResponse_DeserializesAllFieldsWithCorrectTypes()
    {
        var json = "{\"object\":\"customerFiscalInfo\",\"email\":\"john.doe@asaas.com.br\",\"municipalInscription\":\"21779501\",\"simplesNacional\":false,\"culturalProjectsPromoter\":false,\"cnae\":\"6209100\",\"specialTaxRegime\":\"1\",\"nbsCode\":\"1.0101\",\"rpsSerie\":\"1\",\"rpsNumber\":1,\"loteNumber\":1,\"username\":\"johndoe\",\"passwordSent\":true,\"accessTokenSent\":true,\"certificateSent\":true,\"nationalPortalTaxCalculationRegime\":null}";

        var result = JsonContractAssert.DeserializeFixture<FiscalInfo>(json);

        Assert.Equal("customerFiscalInfo", result.Object);
        Assert.Equal("john.doe@asaas.com.br", result.Email);
        Assert.Equal("21779501", result.MunicipalInscription);
        Assert.False(result.SimplesNacional);
        Assert.Equal("6209100", result.Cnae);
        // B-40a: NbsCode novo
        Assert.Equal("1.0101", result.NbsCode);
        // B-40b: RpsNumber e LoteNumber agora sao int (antes string)
        Assert.Equal(1, result.RpsNumber);
        Assert.Equal(1, result.LoteNumber);
        // B-40c: bools novos
        Assert.True(result.PasswordSent);
        Assert.True(result.AccessTokenSent);
        Assert.True(result.CertificateSent);
    }
}
