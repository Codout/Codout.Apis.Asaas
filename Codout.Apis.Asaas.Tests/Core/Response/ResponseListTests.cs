using System.Net;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.PaymentLink;
using Codout.Apis.Asaas.Models.PaymentLink.Enums;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Pix;
using Codout.Apis.Asaas.Models.Pix.Enums;
using Codout.Apis.Asaas.Models.CustomerFiscalInfo;

namespace Codout.Apis.Asaas.Tests.Core.Response;

public class ResponseListTests
{
    private static string BuildListJson(string dataJson, int totalCount = 1, int limit = 10, int offset = 0, bool hasMore = false)
    {
        return $"{{\"hasMore\":{(hasMore ? "true" : "false")},\"totalCount\":{totalCount},\"limit\":{limit},\"offset\":{offset},\"data\":{dataJson}}}";
    }

    #region Success Scenarios

    [Fact]
    public void Constructor_WithOkStatus_DeserializesData()
    {
        var json = BuildListJson("[{\"id\":\"pl_1\",\"name\":\"Link 1\",\"billingType\":\"BOLETO\",\"chargeType\":\"DETACHED\"},{\"id\":\"pl_2\",\"name\":\"Link 2\",\"billingType\":\"PIX\",\"chargeType\":\"RECURRENT\"}]", totalCount: 2);

        var response = new ResponseList<PaymentLink>(HttpStatusCode.OK, json);

        Assert.NotNull(response.Data);
        Assert.Equal(2, response.Data.Count);
        Assert.Equal("pl_1", response.Data[0].Id);
        Assert.Equal("Link 1", response.Data[0].Name);
        Assert.Equal(BillingType.BOLETO, response.Data[0].BillingType);
        Assert.Equal("pl_2", response.Data[1].Id);
        Assert.Equal(BillingType.PIX, response.Data[1].BillingType);
    }

    [Fact]
    public void Constructor_WithOkStatus_ParsesMetadata()
    {
        var json = BuildListJson("[{\"id\":\"pl_1\"}]", totalCount: 100, limit: 20, offset: 40, hasMore: true);

        var response = new ResponseList<PaymentLink>(HttpStatusCode.OK, json);

        Assert.Equal(100, response.TotalCount);
        Assert.Equal(20, response.Limit);
        Assert.Equal(40, response.Offset);
        Assert.True(response.HasMore);
    }

    [Fact]
    public void Constructor_WithOkStatus_HasMoreFalse()
    {
        var json = BuildListJson("[{\"id\":\"pl_1\"}]", totalCount: 1, limit: 10, offset: 0, hasMore: false);

        var response = new ResponseList<PaymentLink>(HttpStatusCode.OK, json);

        Assert.False(response.HasMore);
    }

    [Fact]
    public void Constructor_WithOkStatus_EmptyData()
    {
        var json = BuildListJson("[]", totalCount: 0);

        var response = new ResponseList<PaymentLink>(HttpStatusCode.OK, json);

        Assert.NotNull(response.Data);
        Assert.Empty(response.Data);
        Assert.Equal(0, response.TotalCount);
    }

    [Fact]
    public void Constructor_WithOkStatus_IsSuccessful()
    {
        var json = BuildListJson("[]");

        var response = new ResponseList<PaymentLink>(HttpStatusCode.OK, json);

        Assert.True(response.WasSucessfull());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public void Constructor_WithOkStatus_StoresRawResponse()
    {
        var json = BuildListJson("[{\"id\":\"pl_1\"}]");

        var response = new ResponseList<PaymentLink>(HttpStatusCode.OK, json);

        Assert.Equal(json, response.AsaasResponse);
    }

    [Fact]
    public void Constructor_WithOkStatus_DeserializesEnumsInList()
    {
        var json = BuildListJson("[{\"id\":\"tx_1\",\"status\":\"PENDING\",\"value\":100},{\"id\":\"tx_2\",\"status\":\"DONE\",\"value\":200}]", totalCount: 2);

        var response = new ResponseList<PixTransaction>(HttpStatusCode.OK, json);

        Assert.Equal(2, response.Data.Count);
        Assert.Equal(PixTransactionStatus.PENDING, response.Data[0].Status);
        Assert.Equal(PixTransactionStatus.DONE, response.Data[1].Status);
    }

    [Fact]
    public void Constructor_WithOkStatus_DeserializesMunicipalOptions()
    {
        var json = BuildListJson("[{\"id\":\"opt_1\",\"label\":\"Option A\"},{\"id\":\"opt_2\",\"label\":\"Option B\"}]", totalCount: 2);

        var response = new ResponseList<MunicipalOption>(HttpStatusCode.OK, json);

        Assert.Equal(2, response.Data.Count);
        Assert.Equal("opt_1", response.Data[0].Id);
        Assert.Equal("Option A", response.Data[0].Label);
        Assert.Equal("opt_2", response.Data[1].Id);
        Assert.Equal("Option B", response.Data[1].Label);
    }

    #endregion

    #region Error Scenarios

    [Fact]
    public void Constructor_WithBadRequest_DoesNotDeserializeData()
    {
        var json = "{\"errors\":[{\"code\":\"invalid\",\"description\":\"Bad request\"}]}";

        var response = new ResponseList<PaymentLink>(HttpStatusCode.BadRequest, json);

        Assert.Null(response.Data);
    }

    [Fact]
    public void Constructor_WithBadRequest_IsNotSuccessful()
    {
        var json = "{\"errors\":[{\"code\":\"invalid\",\"description\":\"Bad request\"}]}";

        var response = new ResponseList<PaymentLink>(HttpStatusCode.BadRequest, json);

        Assert.False(response.WasSucessfull());
    }

    [Fact]
    public void Constructor_WithBadRequest_ParsesErrors()
    {
        var json = "{\"errors\":[{\"code\":\"invalid_param\",\"description\":\"Invalid offset\"}]}";

        var response = new ResponseList<PaymentLink>(HttpStatusCode.BadRequest, json);

        Assert.Single(response.Errors);
        Assert.Equal("invalid_param", response.Errors[0].Code);
        Assert.Equal("Invalid offset", response.Errors[0].Description);
    }

    [Fact]
    public void Constructor_WithNotFound_SetsCorrectStatusCode()
    {
        var json = "{\"errors\":[{\"code\":\"not_found\",\"description\":\"Resource not found\"}]}";

        var response = new ResponseList<PaymentLink>(HttpStatusCode.NotFound, json);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public void Constructor_WithErrorAndNonJsonContent_CreatesUnknownError()
    {
        var content = "Server Error";

        var response = new ResponseList<PaymentLink>(HttpStatusCode.InternalServerError, content);

        Assert.NotEmpty(response.Errors);
        Assert.Equal("UnknownError", response.Errors[0].Code);
    }

    #endregion

    #region Metadata Defaults

    [Fact]
    public void Constructor_WithError_MetadataDefaults()
    {
        var json = "{\"errors\":[{\"code\":\"err\",\"description\":\"Error\"}]}";

        var response = new ResponseList<PaymentLink>(HttpStatusCode.BadRequest, json);

        // When not successful, the metadata properties stay at default values
        Assert.False(response.HasMore);
        Assert.Equal(0, response.TotalCount);
        Assert.Equal(0, response.Limit);
        Assert.Equal(0, response.Offset);
    }

    #endregion
}
