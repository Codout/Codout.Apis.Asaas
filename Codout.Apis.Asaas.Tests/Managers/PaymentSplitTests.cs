using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.Payment;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class PaymentSplitTests : ManagerTestBase<PaymentManager>
{
    private static readonly string[] Methods =
        ["FindPaidSplit", "ListPaidSplits", "FindReceivedSplit", "ListReceivedSplits"];

    public static IEnumerable<object[]> CompatiblePayments()
        => Methods.SelectMany(method => new[]
        {
            new object[] { method, "\"pay_test\"", "pay_test" },
            new object[] { method, "null", null! },
            new object[] { method, "absent", null! },
            new object[] { method, "{}", null! },
            new object[] { method, "\"\"", "" },
            new object[] { method, "{\"id\":\"pay_test\",\"confirmedDate\":null,\"invoiceNumber\":null,\"externalReference\":null}", "pay_test" }
        });

    public static IEnumerable<object[]> InvalidPayments()
        => Methods.SelectMany(method => new[] { "123", "12.34", "true", "false", "[]", "[{}]" }
            .Select(payment => new object[] { method, payment }));

    public static IEnumerable<object[]> InvalidPaymentMetadata()
        => Methods.SelectMany(method => new[]
        {
            "{\"id\":123}", "{\"confirmedDate\":\"not-a-date\"}",
            "{\"invoiceNumber\":false}", "{\"externalReference\":[]}"
        }.Select(payment => new object[] { method, payment }));

    protected override PaymentManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestablePaymentManager(settings, handler);

    private static string ObjectResponse => File.ReadAllText(
        Path.Combine(AppContext.BaseDirectory, "Fixtures", "Payment", "split-object.json"));

    private static string WithPayment(string payment)
    {
        var json = JsonNode.Parse(ObjectResponse)!.AsObject();
        if (payment == "absent")
            json.Remove("payment");
        else
            json["payment"] = JsonNode.Parse(payment);
        return json.ToJsonString();
    }

    private async Task<PaymentSplitView> QuerySplit(string method, string json)
    {
        bool list = method.StartsWith("List");
        if (list)
            SetupListResponse<PaymentSplitView>($"[{json}]");
        else
            SetupOkResponse(json);

        PaymentSplitView split;
        switch (method)
        {
            case "FindPaidSplit":
                var paid = await Manager.FindPaidSplit("split_test");
                Assert.True(paid.WasSuccessful());
                split = paid.Data;
                break;
            case "FindReceivedSplit":
                var received = await Manager.FindReceivedSplit("split_test");
                Assert.True(received.WasSuccessful());
                split = received.Data;
                break;
            case "ListPaidSplits":
                var paidList = await Manager.ListPaidSplits(0, 10);
                Assert.True(paidList.WasSuccessful());
                Assert.Equal(1, paidList.TotalCount);
                Assert.Equal(10, paidList.Limit);
                Assert.Equal(0, paidList.Offset);
                Assert.False(paidList.HasMore);
                split = Assert.Single(paidList.Data);
                break;
            case "ListReceivedSplits":
                var receivedList = await Manager.ListReceivedSplits(0, 10);
                Assert.True(receivedList.WasSuccessful());
                Assert.Equal(1, receivedList.TotalCount);
                Assert.Equal(10, receivedList.Limit);
                Assert.Equal(0, receivedList.Offset);
                Assert.False(receivedList.HasMore);
                split = Assert.Single(receivedList.Data);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(method));
        }

        var category = method.Contains("Received") ? "received" : "paid";
        AssertRequestMethod(HttpMethod.Get);
        AssertRequestUrl($"/v3/payments/splits/{category}" + (list ? "?offset=0&limit=10" : "/split_test"));
        Assert.Null(Handler.LastRequestContent);
        return split;
    }

    [Theory]
    [InlineData("FindPaidSplit")]
    [InlineData("ListPaidSplits")]
    [InlineData("FindReceivedSplit")]
    [InlineData("ListReceivedSplits")]
    public async Task CurrentObjectResponse_DeserializesThroughManager(string method)
    {
        var split = await QuerySplit(method, ObjectResponse);

        Assert.Equal("pay_test", split.Payment);
        Assert.NotNull(split.PaymentInfo);
        Assert.Equal("pay_test", split.PaymentInfo.Id);
        Assert.Equal(new DateTime(2026, 10, 8), split.PaymentInfo.ConfirmedDate);
        Assert.Equal("00110291", split.PaymentInfo.InvoiceNumber);
        Assert.Equal("pedido_test", split.PaymentInfo.ExternalReference);
        Assert.Equal("split_test", split.Id);
        Assert.Equal("wallet_test", split.WalletId);
        Assert.Equal(80.25m, split.FixedValue);
        Assert.Null(split.PercentualValue);
        Assert.Equal(80.25m, split.TotalValue);
        Assert.Equal("DONE", split.Status);
        Assert.Null(split.CancellationReason);
        Assert.Equal(new DateTime(2026, 10, 8), split.CreditDate);
        Assert.Equal("split_reference", split.ExternalReference);
        Assert.Equal("Split de teste", split.Description);
        Assert.Equal("account_origin", split.OriginAccountId);
        Assert.Equal("account_destination", split.DestinationAccountId);
    }

    [Theory]
    [MemberData(nameof(CompatiblePayments))]
    public async Task LegacyNullMissingAndPartialPayment_Deserializes(string method, string payment, string? id)
    {
        var split = await QuerySplit(method, WithPayment(payment));

        Assert.Equal(id, split.Payment);
        if (payment is "null" or "absent")
            Assert.Null(split.PaymentInfo);
        else
        {
            Assert.NotNull(split.PaymentInfo);
            Assert.Equal(id, split.PaymentInfo.Id);
            Assert.Null(split.PaymentInfo.ConfirmedDate);
            Assert.Null(split.PaymentInfo.InvoiceNumber);
            Assert.Null(split.PaymentInfo.ExternalReference);
        }
    }

    [Theory]
    [InlineData("FindPaidSplit")]
    [InlineData("ListPaidSplits")]
    [InlineData("FindReceivedSplit")]
    [InlineData("ListReceivedSplits")]
    public async Task RefundedSplit_PreservesZeroAndDecimalValues(string method)
    {
        var json = JsonNode.Parse(ObjectResponse)!;
        json["status"] = "REFUNDED";
        json["cancellationReason"] = "PAYMENT_REFUNDED";
        json["fixedValue"] = 80.123456789m;
        json["percentualValue"] = 12.3456789m;
        json["totalValue"] = 0.00m;

        var split = await QuerySplit(method, json.ToJsonString());

        Assert.Equal("REFUNDED", split.Status);
        Assert.Equal("PAYMENT_REFUNDED", split.CancellationReason);
        Assert.Equal(80.123456789m, split.FixedValue);
        Assert.Equal(12.3456789m, split.PercentualValue);
        Assert.Equal(0.00m, split.TotalValue);
    }

    [Theory]
    [InlineData("FindPaidSplit")]
    [InlineData("ListPaidSplits")]
    [InlineData("FindReceivedSplit")]
    [InlineData("ListReceivedSplits")]
    public async Task UnknownPropertiesAndCaseInsensitiveNames_AreSupported(string method)
    {
        var json = JsonNode.Parse(ObjectResponse)!.AsObject();
        json["futureSplitField"] = JsonNode.Parse("{\"nested\":[true,1]}");
        json["payment"]!["futurePaymentField"] = JsonNode.Parse("[{},123]");
        var payment = json["payment"]!.AsObject();
        payment["ID"] = payment["id"]!.DeepClone();
        payment.Remove("id");
        json["PAYMENT"] = payment.DeepClone();
        json.Remove("payment");

        var split = await QuerySplit(method, json.ToJsonString());

        Assert.Equal("pay_test", split.Payment);
        Assert.Equal("00110291", split.PaymentInfo.InvoiceNumber);
        Assert.Equal(80.25m, split.TotalValue);
    }

    [Theory]
    [MemberData(nameof(InvalidPayments))]
    public async Task InvalidPaymentToken_ThrowsClearJsonException(string method, string payment)
    {
        var error = await Assert.ThrowsAsync<JsonException>(() => QuerySplit(method, WithPayment(payment)));

        Assert.Contains("Invalid payment token", error.Message);
        Assert.Contains("expected an object, a string ID, or null", error.Message);
        Assert.EndsWith(".payment", error.Path);
    }

    [Theory]
    [MemberData(nameof(InvalidPaymentMetadata))]
    public async Task InvalidTypedMetadata_ThrowsJsonException(string method, string payment)
    {
        await Assert.ThrowsAsync<JsonException>(() => QuerySplit(method, WithPayment(payment)));
    }

    [Fact]
    public async Task PaymentStringGetterAndSetter_RemainCompatible()
    {
        var split = await QuerySplit("FindPaidSplit", ObjectResponse);
        string paymentId = split.Payment;
        Assert.Equal("pay_test", paymentId);

        split.PaymentInfo.Id = "pay_updated";
        Assert.Equal("pay_updated", split.Payment);
        split.Payment = "pay_legacy";
        Assert.Equal("pay_legacy", split.Payment);
        Assert.Equal("pay_legacy", split.PaymentInfo.Id);
        Assert.Null(split.PaymentInfo.InvoiceNumber);
        Assert.Null(split.PaymentInfo.ConfirmedDate);
        Assert.Null(split.PaymentInfo.ExternalReference);
        split.Payment = null!;
        Assert.Null(split.Payment);
        Assert.Null(split.PaymentInfo);
        split.PaymentInfo = new PaymentSplitPaymentInfo { Id = "pay_typed", InvoiceNumber = "00001" };
        Assert.Equal("pay_typed", split.Payment);
        split.PaymentInfo = null!;
        Assert.Null(split.Payment);
    }

    [Fact]
    public async Task ObjectSerialization_RoundTripsAllKnownFieldsWithoutDuplicatePayment()
    {
        var original = await QuerySplit("FindPaidSplit", ObjectResponse);
        original.CancellationReason = "PAYMENT_REFUNDED";
        original.PercentualValue = 12.345m;
        var json = JsonSerializer.Serialize(original, JsonSerializerConfiguration.Options);
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        Assert.Equal(1, root.EnumerateObject().Count(p => p.Name == "payment"));
        Assert.False(root.TryGetProperty("paymentInfo", out _));
        Assert.Equal(13, root.EnumerateObject().Count());
        Assert.Equal(4, root.GetProperty("payment").EnumerateObject().Count());
        Assert.Equal("2026-10-08", root.GetProperty("creditDate").GetString());
        Assert.Equal("2026-10-08", root.GetProperty("payment").GetProperty("confirmedDate").GetString());
        var roundTrip = await QuerySplit("ListReceivedSplits", json);
        Assert.Equal(json, JsonSerializer.Serialize(roundTrip, JsonSerializerConfiguration.Options));
    }

    [Theory]
    [InlineData("\"pay_test\"", JsonValueKind.String)]
    [InlineData("\"\"", JsonValueKind.String)]
    [InlineData("{\"id\":\"pay_test\"}", JsonValueKind.String)]
    [InlineData("{}", JsonValueKind.Object)]
    [InlineData("{\"invoiceNumber\":\"00001\"}", JsonValueKind.Object)]
    [InlineData("{\"id\":\"pay_test\",\"externalReference\":\"\"}", JsonValueKind.Object)]
    public async Task PaymentSerialization_PreservesReferenceOrMetadata(string payment, JsonValueKind expectedKind)
    {
        var split = await QuerySplit("FindReceivedSplit", WithPayment(payment));
        var json = JsonSerializer.Serialize(split, JsonSerializerConfiguration.Options);
        using var document = JsonDocument.Parse(json);
        Assert.Equal(expectedKind, document.RootElement.GetProperty("payment").ValueKind);
        Assert.False(document.RootElement.TryGetProperty("paymentInfo", out _));
        var roundTrip = await QuerySplit("ListPaidSplits", json);
        Assert.Equal(split.Payment, roundTrip.Payment);
        Assert.Equal(split.PaymentInfo.InvoiceNumber, roundTrip.PaymentInfo.InvoiceNumber);
        Assert.Equal(split.PaymentInfo.ExternalReference, roundTrip.PaymentInfo.ExternalReference);
    }

    [Fact]
    public void LegacyAssignment_SerializesAsStringWithRealOptions()
    {
        var split = new PaymentSplitView { Id = "split_legacy", Payment = "pay_legacy", TotalValue = 0m };
        var json = JsonSerializer.Serialize(split, JsonSerializerConfiguration.Options);
        using var document = JsonDocument.Parse(json);
        Assert.Equal("pay_legacy", document.RootElement.GetProperty("payment").GetString());
        Assert.Equal(3, document.RootElement.EnumerateObject().Count());
    }

    [Theory]
    [InlineData("null")]
    [InlineData("absent")]
    public async Task NullPaymentSerialization_UsesSdkNullOmission(string payment)
    {
        var split = await QuerySplit("FindPaidSplit", WithPayment(payment));
        var json = JsonSerializer.Serialize(split, JsonSerializerConfiguration.Options);
        using var document = JsonDocument.Parse(json);
        Assert.False(document.RootElement.TryGetProperty("payment", out _));
        Assert.False(document.RootElement.TryGetProperty("paymentInfo", out _));
        var roundTrip = await QuerySplit("ListReceivedSplits", json);
        Assert.Null(roundTrip.Payment);
        Assert.Null(roundTrip.PaymentInfo);
    }
}
