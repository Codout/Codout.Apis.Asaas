using Codout.Apis.Asaas.Models.Checkout;
using Codout.Apis.Asaas.Models.Checkout.Enums;
using Codout.Apis.Asaas.Models.Subscription.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para CheckoutManager.
/// Schema verificado via MCP em 2026-05-24.
/// </summary>
public class CheckoutContractTests
{
    // ─────────────────────────────────────────────────────────────
    // POST /v3/checkouts (request)
    // Required: billingTypes[], chargeTypes[], callback, items[]
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void CreateCheckoutRequest_Minimal_HasAllRequiredKeys()
    {
        var request = new CreateCheckoutRequest
        {
            BillingTypes = [CheckoutBillingType.CREDIT_CARD],
            ChargeTypes = [CheckoutChargeType.DETACHED],
            Callback = new CheckoutCallback
            {
                SuccessUrl = "https://example.com/asaas/checkout/success",
                CancelUrl = "https://example.com/asaas/checkout/cancel"
            },
            Items = [new CheckoutItem { Name = "Roupas", Quantity = 2, Value = 100m, ImageBase64 = "IMAGE IN BASE64" }]
        };

        JsonContractAssert.SerializesWithKeys(request, "billingTypes", "chargeTypes", "callback", "items");
    }

    [Fact]
    public void CreateCheckoutRequest_UsesSplitsPlural_NotSingular()
    {
        var request = new CreateCheckoutRequest
        {
            BillingTypes = [CheckoutBillingType.PIX],
            ChargeTypes = [CheckoutChargeType.DETACHED],
            Callback = new CheckoutCallback { SuccessUrl = "x", CancelUrl = "x" },
            Items = [new CheckoutItem { Name = "X", Quantity = 1, Value = 1m, ImageBase64 = "X" }],
            Splits = [new CheckoutSplit { WalletId = "w1", PercentageValue = 10m }]
        };

        // Quirk Asaas: request usa "splits" (plural), response usa "split" (singular).
        // Documentado em CreateCheckoutRequest.cs.
        JsonContractAssert.SerializesWithKeys(request, "splits");
        JsonContractAssert.DoesNotSerializeKey(request, "split");
    }

    [Fact]
    public void CreateCheckoutRequest_NoFakeFields()
    {
        var request = new CreateCheckoutRequest
        {
            BillingTypes = [CheckoutBillingType.PIX],
            ChargeTypes = [CheckoutChargeType.DETACHED],
            Callback = new CheckoutCallback { SuccessUrl = "x", CancelUrl = "x" },
            Items = [new CheckoutItem { Name = "X", Quantity = 1, Value = 1m, ImageBase64 = "X" }]
        };

        // Regressao B-04: campos inventados (value/dueDate/customer/description)
        // que estavam no model antigo e nao existem na API.
        JsonContractAssert.DoesNotSerializeKey(request, "value");
        JsonContractAssert.DoesNotSerializeKey(request, "dueDate");
        JsonContractAssert.DoesNotSerializeKey(request, "customer");
        JsonContractAssert.DoesNotSerializeKey(request, "description");
        JsonContractAssert.DoesNotSerializeKey(request, "checkoutUrl");
    }

    // ─────────────────────────────────────────────────────────────
    // POST /v3/checkouts and POST /v3/checkouts/{id}/cancel
    // Both return CheckoutSessionResponseDTO
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void CheckoutResponse_DeserializesFromOfficialFixture_FullShape()
    {
        var json = FixtureLoader.Load("Checkout/response.json");

        var result = JsonContractAssert.DeserializeFixture<Checkout>(json);

        Assert.Equal("131ca662-56c8-4479-b5b3-fd61a413fce7", result.Id);
        Assert.Equal("https://sandbox.asaas.com/checkoutSession/show/131ca662-56c8-4479-b5b3-fd61a413fce7", result.Link);
        Assert.Equal(CheckoutStatus.ACTIVE, result.Status);

        Assert.Contains(CheckoutBillingType.CREDIT_CARD, result.BillingTypes);
        Assert.Contains(CheckoutChargeType.RECURRENT, result.ChargeTypes);

        Assert.Equal(100, result.MinutesToExpire);
        Assert.Equal("dcf4dff9-b080-425c-b234-765f2ffac0ae", result.ExternalReference);

        Assert.NotNull(result.Callback);
        Assert.Equal("https://example.com/asaas/checkout/success", result.Callback.SuccessUrl);
        Assert.Equal("https://example.com/asaas/checkout/cancel", result.Callback.CancelUrl);
        Assert.Equal("https://example.com/asaas/checkout/expired", result.Callback.ExpiredUrl);

        Assert.Single(result.Items);
        Assert.Equal("Roupas", result.Items[0].Name);
        Assert.Equal(2, result.Items[0].Quantity);
        Assert.Equal(100m, result.Items[0].Value);

        Assert.NotNull(result.CustomerData);
        Assert.Equal("John Doe", result.CustomerData.Name);
        Assert.Equal(150, result.CustomerData.AddressNumber);
        Assert.Equal(12987382, result.CustomerData.City);

        Assert.NotNull(result.Subscription);
        Assert.Equal(Cycle.MONTHLY, result.Subscription.Cycle);
    }

    [Fact]
    public void CheckoutResponse_UsesSplitSingular_OnResponse()
    {
        // Asaas: response usa "split" singular (contrario ao request "splits")
        var json = "{\"id\":\"ck_1\",\"split\":[{\"walletId\":\"w1\",\"fixedValue\":10}]}";

        var result = JsonContractAssert.DeserializeFixture<Checkout>(json);

        Assert.Single(result.Split);
        Assert.Equal("w1", result.Split[0].WalletId);
        Assert.Equal(10m, result.Split[0].FixedValue);
    }

    [Fact]
    public void CheckoutStatus_AllValuesDeserialize()
    {
        // Schema: ACTIVE, CANCELED, EXPIRED, PAID
        foreach (var status in new[] { "ACTIVE", "CANCELED", "EXPIRED", "PAID" })
        {
            var json = $"{{\"id\":\"x\",\"status\":\"{status}\"}}";
            var result = JsonContractAssert.DeserializeFixture<Checkout>(json);
            Assert.Equal(status, result.Status.ToString());
        }
    }
}
