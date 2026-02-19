using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.CustomerFiscalInfo;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class CustomerFiscalInfoManagerTests : ManagerTestBase<CustomerFiscalInfoManager>
{
    protected override CustomerFiscalInfoManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableCustomerFiscalInfoManager(settings, handler);

    #region Find

    [Fact]
    public async Task Find_SendsGetRequest()
    {
        SetupOkResponse("{\"email\":\"test@example.com\",\"municipalInscription\":\"12345\",\"stateInscription\":\"67890\",\"simplesNacional\":true,\"culturalProjectsPromoter\":false,\"cnae\":\"6201-5/00\",\"specialTaxRegime\":\"MICROEMPRESA\",\"serviceListItem\":\"14.01\",\"rpsSerie\":\"A\",\"rpsNumber\":\"100\",\"loteNumber\":\"1\",\"username\":\"testuser\",\"accessToken\":\"token123\"}");

        var result = await Manager.Find();

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/customerFiscalInfo");
        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("test@example.com", result.Data.Email);
        Assert.Equal("12345", result.Data.MunicipalInscription);
        Assert.Equal("67890", result.Data.StateInscription);
        Assert.True(result.Data.SimplesNacional);
        Assert.False(result.Data.CulturalProjectsPromoter);
        Assert.Equal("6201-5/00", result.Data.Cnae);
        Assert.Equal("MICROEMPRESA", result.Data.SpecialTaxRegime);
        Assert.Equal("14.01", result.Data.ServiceListItem);
        Assert.Equal("A", result.Data.RpsSerie);
        Assert.Equal("100", result.Data.RpsNumber);
        Assert.Equal("1", result.Data.LoteNumber);
        Assert.Equal("testuser", result.Data.Username);
        Assert.Equal("token123", result.Data.AccessToken);
    }

    #endregion

    #region ListMunicipalOptions

    [Fact]
    public async Task ListMunicipalOptions_SendsGetRequest()
    {
        SetupListResponse<MunicipalOption>("[{\"id\":\"mo_1\",\"label\":\"Sao Paulo\"},{\"id\":\"mo_2\",\"label\":\"Rio de Janeiro\"}]", totalCount: 2, limit: 100, offset: 0);

        var result = await Manager.ListMunicipalOptions();

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/customerFiscalInfo/municipalOptions");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=100");
        Assert.True(result.WasSucessfull());
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("mo_1", result.Data[0].Id);
        Assert.Equal("Sao Paulo", result.Data[0].Label);
        Assert.Equal("mo_2", result.Data[1].Id);
        Assert.Equal("Rio de Janeiro", result.Data[1].Label);
    }

    [Fact]
    public async Task ListMunicipalOptions_ParsesListMetadata()
    {
        SetupListResponse<MunicipalOption>("[{\"id\":\"mo_1\",\"label\":\"Test\"}]", totalCount: 50, limit: 100, offset: 0, hasMore: false);

        var result = await Manager.ListMunicipalOptions();

        Assert.Equal(50, result.TotalCount);
        Assert.Equal(100, result.Limit);
        Assert.Equal(0, result.Offset);
        Assert.False(result.HasMore);
    }

    #endregion

    #region Error Handling

    [Fact]
    public async Task Find_ReturnsErrorOnUnauthorized()
    {
        Handler.WithResponse(HttpStatusCode.Unauthorized, "{\"errors\":[{\"code\":\"unauthorized\",\"description\":\"Invalid API key\"}]}");

        var result = await Manager.Find();

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("unauthorized", result.Errors[0].Code);
    }

    [Fact]
    public async Task Find_ReturnsNullDataOnError()
    {
        SetupErrorResponse(HttpStatusCode.Forbidden);

        var result = await Manager.Find();

        Assert.Null(result.Data);
    }

    #endregion
}
