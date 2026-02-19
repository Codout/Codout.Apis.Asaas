using System.Net;
using System.Net.Http;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.Pix;
using Codout.Apis.Asaas.Models.Pix.Enums;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class PixManagerTests : ManagerTestBase<PixManager>
{
    protected override PixManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestablePixManager(settings, handler);

    #region ListTransactions

    [Fact]
    public async Task ListTransactions_SendsGetRequest()
    {
        SetupListResponse<PixTransaction>("[{\"id\":\"pix_tx_1\",\"payment\":\"pay_123\",\"status\":\"DONE\",\"value\":150.75,\"description\":\"Test payment\"}]", totalCount: 1, limit: 10, offset: 0);

        var result = await Manager.ListTransactions(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/pix/transactions");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
        Assert.True(result.WasSucessfull());
        Assert.Single(result.Data);
        Assert.Equal("pix_tx_1", result.Data[0].Id);
        Assert.Equal("pay_123", result.Data[0].Payment);
        Assert.Equal(PixTransactionStatus.DONE, result.Data[0].Status);
        Assert.Equal(150.75m, result.Data[0].Value);
        Assert.Equal("Test payment", result.Data[0].Description);
    }

    [Fact]
    public async Task ListTransactions_WithPagination()
    {
        SetupListResponse<PixTransaction>("[]", totalCount: 100, limit: 20, offset: 40, hasMore: true);

        var result = await Manager.ListTransactions(40, 20);

        AssertRequestUrlContains("offset=40");
        AssertRequestUrlContains("limit=20");
        Assert.True(result.HasMore);
        Assert.Equal(100, result.TotalCount);
    }

    #endregion

    #region CancelTransaction

    [Fact]
    public async Task CancelTransaction_SendsPostRequest()
    {
        SetupOkResponse("{\"id\":\"pix_tx_cancel\",\"payment\":\"pay_456\",\"status\":\"CANCELLED\",\"value\":50.00}");

        var result = await Manager.CancelTransaction("pix_tx_cancel");

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/pix/transactions/pix_tx_cancel/cancel");
        Assert.True(result.WasSucessfull());
        Assert.Equal("pix_tx_cancel", result.Data.Id);
        Assert.Equal(PixTransactionStatus.CANCELLED, result.Data.Status);
    }

    #endregion

    #region CreateStaticQrCode

    [Fact]
    public async Task CreateStaticQrCode_SendsPostRequest()
    {
        SetupOkResponse("{\"id\":\"qr_123\",\"encodedImage\":\"base64data\",\"payload\":\"00020126...\",\"allowsMultiplePayments\":true}");

        var request = new CreatePixStaticQrCodeRequest
        {
            AddressKey = "key_abc",
            Description = "QR Code Test",
            Value = 25.50m
        };

        var result = await Manager.CreateStaticQrCode(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/pix/qrCodes/static");
        Assert.True(result.WasSucessfull());
        Assert.Equal("qr_123", result.Data.Id);
        Assert.Equal("base64data", result.Data.EncodedImage);
        Assert.Equal("00020126...", result.Data.Payload);
        Assert.True(result.Data.AllowsMultiplePayments);
    }

    [Fact]
    public async Task CreateStaticQrCode_SerializesRequestBody()
    {
        SetupOkResponse("{\"id\":\"qr_123\"}");

        var request = new CreatePixStaticQrCodeRequest
        {
            AddressKey = "my_key",
            Description = "Test",
            Value = 100.00m
        };

        await Manager.CreateStaticQrCode(request);

        Assert.NotNull(Handler.LastRequestContent);
        Assert.Contains("\"addressKey\":\"my_key\"", Handler.LastRequestContent);
        Assert.Contains("\"description\":\"Test\"", Handler.LastRequestContent);
        Assert.Contains("\"value\":100", Handler.LastRequestContent);
    }

    #endregion

    #region DecodeQrCode

    [Fact]
    public async Task DecodeQrCode_SendsPostRequest()
    {
        SetupOkResponse("{\"payload\":\"00020126...\",\"type\":\"STATIC\",\"endToEndIdentifier\":\"E123\",\"originalValue\":99.99,\"receiverName\":\"John Doe\"}");

        var request = new DecodePixQrCodeRequest
        {
            Payload = "00020126..."
        };

        var result = await Manager.DecodeQrCode(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/pix/qrCodes/decode");
        Assert.True(result.WasSucessfull());
        Assert.Equal("00020126...", result.Data.Payload);
        Assert.Equal("STATIC", result.Data.Type);
        Assert.Equal("E123", result.Data.EndToEndIdentifier);
        Assert.Equal(99.99m, result.Data.OriginalValue);
        Assert.Equal("John Doe", result.Data.ReceiverName);
    }

    #endregion

    #region PayQrCode

    [Fact]
    public async Task PayQrCode_SendsPostRequest()
    {
        SetupOkResponse("{\"id\":\"pix_pay_123\",\"value\":75.00,\"description\":\"QR payment\",\"status\":\"PENDING\"}");

        var request = new PayPixQrCodeRequest
        {
            QrCode = new PayPixQrCodeInfo { Payload = "00020126..." },
            Value = 75.00m,
            Description = "QR payment"
        };

        var result = await Manager.PayQrCode(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/pix/qrCodes/pay");
        Assert.True(result.WasSucessfull());
        Assert.Equal("pix_pay_123", result.Data.Id);
        Assert.Equal(75.00m, result.Data.Value);
        Assert.Equal("QR payment", result.Data.Description);
        Assert.Equal("PENDING", result.Data.Status);
    }

    [Fact]
    public async Task PayQrCode_SerializesNestedQrCodeObject()
    {
        SetupOkResponse("{\"id\":\"pix_pay_123\"}");

        var request = new PayPixQrCodeRequest
        {
            QrCode = new PayPixQrCodeInfo { Payload = "testpayload" },
            Value = 50.00m,
            Description = "Nested test"
        };

        await Manager.PayQrCode(request);

        Assert.NotNull(Handler.LastRequestContent);
        Assert.Contains("\"qrCode\":", Handler.LastRequestContent);
        Assert.Contains("\"payload\":\"testpayload\"", Handler.LastRequestContent);
    }

    #endregion

    #region CreateAddressKey

    [Fact]
    public async Task CreateAddressKey_SendsPostRequest()
    {
        SetupOkResponse("{\"id\":\"key_123\",\"key\":\"12345678901\",\"type\":\"CPF\",\"status\":\"ACTIVE\",\"dateCreated\":\"2024-01-15\"}");

        var request = new CreatePixAddressKeyRequest
        {
            Type = PixAddressKeyType.CPF
        };

        var result = await Manager.CreateAddressKey(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/pix/addressKeys");
        Assert.True(result.WasSucessfull());
        Assert.Equal("key_123", result.Data.Id);
        Assert.Equal("12345678901", result.Data.Key);
        Assert.Equal(PixAddressKeyType.CPF, result.Data.Type);
        Assert.Equal("ACTIVE", result.Data.Status);
    }

    [Fact]
    public async Task CreateAddressKey_SerializesEnumAsString()
    {
        SetupOkResponse("{\"id\":\"key_123\"}");

        var request = new CreatePixAddressKeyRequest { Type = PixAddressKeyType.EVP };

        await Manager.CreateAddressKey(request);

        Assert.NotNull(Handler.LastRequestContent);
        Assert.Contains("\"type\":\"EVP\"", Handler.LastRequestContent);
    }

    [Theory]
    [InlineData(PixAddressKeyType.CPF)]
    [InlineData(PixAddressKeyType.CNPJ)]
    [InlineData(PixAddressKeyType.EMAIL)]
    [InlineData(PixAddressKeyType.PHONE)]
    [InlineData(PixAddressKeyType.EVP)]
    public async Task CreateAddressKey_AllKeyTypes(PixAddressKeyType keyType)
    {
        SetupOkResponse($"{{\"id\":\"key_123\",\"type\":\"{keyType}\"}}");

        var request = new CreatePixAddressKeyRequest { Type = keyType };
        var result = await Manager.CreateAddressKey(request);

        Assert.Equal(keyType, result.Data.Type);
    }

    #endregion

    #region ListAddressKeys

    [Fact]
    public async Task ListAddressKeys_SendsGetRequest()
    {
        SetupListResponse<PixAddressKey>("[{\"id\":\"key_1\",\"key\":\"test@email.com\",\"type\":\"EMAIL\",\"status\":\"ACTIVE\",\"dateCreated\":\"2024-03-01\"}]", totalCount: 1, limit: 10, offset: 0);

        var result = await Manager.ListAddressKeys(0, 10);

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrlContains("/v3/pix/addressKeys");
        AssertRequestUrlContains("offset=0");
        AssertRequestUrlContains("limit=10");
        Assert.True(result.WasSucessfull());
        Assert.Single(result.Data);
        Assert.Equal("key_1", result.Data[0].Id);
        Assert.Equal(PixAddressKeyType.EMAIL, result.Data[0].Type);
    }

    #endregion

    #region FindAddressKey

    [Fact]
    public async Task FindAddressKey_SendsGetRequest()
    {
        SetupOkResponse("{\"id\":\"key_find\",\"key\":\"00000000000\",\"type\":\"CPF\",\"status\":\"ACTIVE\",\"dateCreated\":\"2024-05-01\"}");

        var result = await Manager.FindAddressKey("key_find");

        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl("/v3/pix/addressKeys/key_find");
        Assert.True(result.WasSucessfull());
        Assert.Equal("key_find", result.Data.Id);
    }

    #endregion

    #region DeleteAddressKey

    [Fact]
    public async Task DeleteAddressKey_SendsDeleteRequest()
    {
        // BaseDeleted is abstract and cannot be deserialized by System.Text.Json.
        // Use an error response to verify the correct URL and method are used.
        SetupErrorResponse(HttpStatusCode.NotFound);

        var result = await Manager.DeleteAddressKey("key_del");

        AssertRequestMethod(HttpMethod.Delete);
        AssertRequestUrl("/v3/pix/addressKeys/key_del");
    }

    #endregion

    #region Error Handling

    [Fact]
    public async Task CancelTransaction_ReturnsErrorOnBadRequest()
    {
        SetupErrorResponse(HttpStatusCode.BadRequest);

        var result = await Manager.CancelTransaction("invalid_tx");

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task FindAddressKey_ReturnsNullDataOnError()
    {
        SetupErrorResponse(HttpStatusCode.NotFound);

        var result = await Manager.FindAddressKey("nonexistent");

        Assert.Null(result.Data);
    }

    #endregion
}
