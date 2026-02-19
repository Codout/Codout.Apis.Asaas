using System.Net;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.PaymentLink;
using Codout.Apis.Asaas.Models.PaymentLink.Enums;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Pix;
using Codout.Apis.Asaas.Models.Pix.Enums;

namespace Codout.Apis.Asaas.Tests.Core.Response;

public class ResponseObjectTests
{
    #region Success Scenarios

    [Fact]
    public void Constructor_WithOkStatus_DeserializesData()
    {
        var json = "{\"id\":\"pl_123\",\"name\":\"Test Link\",\"value\":100.50,\"billingType\":\"BOLETO\",\"chargeType\":\"DETACHED\"}";

        var response = new ResponseObject<PaymentLink>(HttpStatusCode.OK, json);

        Assert.NotNull(response.Data);
        Assert.Equal("pl_123", response.Data.Id);
        Assert.Equal("Test Link", response.Data.Name);
        Assert.Equal(100.50m, response.Data.Value);
        Assert.Equal(BillingType.BOLETO, response.Data.BillingType);
        Assert.Equal(ChargeType.DETACHED, response.Data.ChargeType);
    }

    [Fact]
    public void Constructor_WithOkStatus_SetsStatusCode()
    {
        var json = "{\"id\":\"test\"}";

        var response = new ResponseObject<PaymentLink>(HttpStatusCode.OK, json);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public void Constructor_WithOkStatus_IsSuccessful()
    {
        var json = "{\"id\":\"test\"}";

        var response = new ResponseObject<PaymentLink>(HttpStatusCode.OK, json);

        Assert.True(response.WasSucessfull());
    }

    [Fact]
    public void Constructor_WithOkStatus_HasNoErrors()
    {
        var json = "{\"id\":\"test\"}";

        var response = new ResponseObject<PaymentLink>(HttpStatusCode.OK, json);

        Assert.Empty(response.Errors);
    }

    [Fact]
    public void Constructor_WithOkStatus_StoresRawResponse()
    {
        var json = "{\"id\":\"test\",\"name\":\"Hello\"}";

        var response = new ResponseObject<PaymentLink>(HttpStatusCode.OK, json);

        Assert.Equal(json, response.AsaasResponse);
    }

    [Fact]
    public void Constructor_WithOkStatus_DeserializesEnums()
    {
        var json = "{\"id\":\"tx_1\",\"status\":\"DONE\",\"value\":50.0}";

        var response = new ResponseObject<PixTransaction>(HttpStatusCode.OK, json);

        Assert.NotNull(response.Data);
        Assert.Equal(PixTransactionStatus.DONE, response.Data.Status);
    }

    [Fact]
    public void Constructor_WithOkStatus_DeserializesNullableFields()
    {
        var json = "{\"id\":\"pl_1\",\"endDate\":\"2024-12-31T00:00:00\",\"billingType\":\"PIX\",\"chargeType\":\"DETACHED\"}";

        var response = new ResponseObject<PaymentLink>(HttpStatusCode.OK, json);

        Assert.NotNull(response.Data);
        Assert.NotNull(response.Data.EndDate);
    }

    #endregion

    #region Error Scenarios

    [Fact]
    public void Constructor_WithBadRequest_DoesNotDeserializeData()
    {
        var json = "{\"errors\":[{\"code\":\"invalid\",\"description\":\"Test error\"}]}";

        var response = new ResponseObject<PaymentLink>(HttpStatusCode.BadRequest, json);

        Assert.Null(response.Data);
    }

    [Fact]
    public void Constructor_WithBadRequest_IsNotSuccessful()
    {
        var json = "{\"errors\":[{\"code\":\"invalid\",\"description\":\"Test error\"}]}";

        var response = new ResponseObject<PaymentLink>(HttpStatusCode.BadRequest, json);

        Assert.False(response.WasSucessfull());
    }

    [Fact]
    public void Constructor_WithBadRequest_ParsesErrors()
    {
        var json = "{\"errors\":[{\"code\":\"invalid_field\",\"description\":\"Name is required\"}]}";

        var response = new ResponseObject<PaymentLink>(HttpStatusCode.BadRequest, json);

        Assert.Single(response.Errors);
        Assert.Equal("invalid_field", response.Errors[0].Code);
        Assert.Equal("Name is required", response.Errors[0].Description);
    }

    [Fact]
    public void Constructor_WithMultipleErrors_ParsesAllErrors()
    {
        var json = "{\"errors\":[{\"code\":\"err1\",\"description\":\"Error 1\"},{\"code\":\"err2\",\"description\":\"Error 2\"}]}";

        var response = new ResponseObject<PaymentLink>(HttpStatusCode.BadRequest, json);

        Assert.Equal(2, response.Errors.Count);
        Assert.Equal("err1", response.Errors[0].Code);
        Assert.Equal("err2", response.Errors[1].Code);
    }

    [Fact]
    public void Constructor_WithNotFound_SetsCorrectStatusCode()
    {
        var json = "{\"errors\":[{\"code\":\"not_found\",\"description\":\"Not found\"}]}";

        var response = new ResponseObject<PaymentLink>(HttpStatusCode.NotFound, json);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(response.WasSucessfull());
    }

    [Fact]
    public void Constructor_WithInternalServerError_SetsCorrectStatusCode()
    {
        var json = "{\"errors\":[{\"code\":\"server_error\",\"description\":\"Internal error\"}]}";

        var response = new ResponseObject<PaymentLink>(HttpStatusCode.InternalServerError, json);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(response.WasSucessfull());
    }

    [Fact]
    public void Constructor_WithUnauthorized_IsNotSuccessful()
    {
        var json = "{\"errors\":[{\"code\":\"unauthorized\",\"description\":\"Invalid token\"}]}";

        var response = new ResponseObject<PaymentLink>(HttpStatusCode.Unauthorized, json);

        Assert.False(response.WasSucessfull());
    }

    [Fact]
    public void Constructor_WithErrorAndNonJsonContent_CreatesUnknownError()
    {
        var content = "Something went wrong";

        var response = new ResponseObject<PaymentLink>(HttpStatusCode.InternalServerError, content);

        Assert.NotEmpty(response.Errors);
        Assert.Equal("UnknownError", response.Errors[0].Code);
        Assert.Equal(content, response.Errors[0].Description);
    }

    [Fact]
    public void Constructor_WithErrorAndNoErrorsProperty_CreatesUnknownError()
    {
        var json = "{\"message\":\"Something failed\"}";

        // The JSON is valid but doesn't have "errors" property, so Errors list stays empty
        var response = new ResponseObject<PaymentLink>(HttpStatusCode.BadRequest, json);

        Assert.False(response.WasSucessfull());
        Assert.Empty(response.Errors);
    }

    #endregion

    #region Various Status Codes

    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.Created)]
    public void Constructor_With2xxStatus_IsSuccessful(HttpStatusCode statusCode)
    {
        // Note: ResponseObject only deserializes Data on HttpStatusCode.OK (200)
        var json = "{\"id\":\"test\"}";

        var response = new ResponseObject<PaymentLink>(statusCode, json);

        Assert.True(response.WasSucessfull());
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public void Constructor_WithNon2xxStatus_IsNotSuccessful(HttpStatusCode statusCode)
    {
        var json = "{\"errors\":[{\"code\":\"err\",\"description\":\"Error\"}]}";

        var response = new ResponseObject<PaymentLink>(statusCode, json);

        Assert.False(response.WasSucessfull());
    }

    #endregion
}
