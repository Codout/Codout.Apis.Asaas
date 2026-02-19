using System.Text.Json;
using System.Text.Json.Serialization;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.PaymentLink;
using Codout.Apis.Asaas.Models.PaymentLink.Enums;
using Codout.Apis.Asaas.Models.Pix.Enums;

namespace Codout.Apis.Asaas.Tests.Core;

/// <summary>
/// Tests for the JSON serialization configuration used across the SDK.
/// Since JsonSerializerConfiguration is internal, we test its behavior through
/// actual serialization/deserialization of SDK models.
/// </summary>
public class JsonSerializerConfigurationTests
{
    // We cannot access the internal Options directly, so we replicate the same configuration
    // to verify the expected behavior pattern. The actual SDK configuration is validated
    // through integration with models in the serialization tests.
    private static JsonSerializerOptions CreateTestOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }

    #region CamelCase Naming

    [Fact]
    public void Serialization_UsesCamelCaseNaming()
    {
        var options = CreateTestOptions();
        var model = new PaymentLink
        {
            Id = "test",
            Name = "Test Link",
            DueDateLimitDays = 5,
            MaxInstallmentCount = 3,
            NotificationEnabled = true
        };

        var json = JsonSerializer.Serialize(model, options);

        Assert.Contains("\"dueDateLimitDays\":", json);
        Assert.Contains("\"maxInstallmentCount\":", json);
        Assert.Contains("\"notificationEnabled\":", json);
        Assert.DoesNotContain("\"DueDateLimitDays\":", json);
        Assert.DoesNotContain("\"MaxInstallmentCount\":", json);
    }

    #endregion

    #region Enum Serialization

    [Fact]
    public void Serialization_EnumSerializesAsString()
    {
        var options = CreateTestOptions();
        var model = new PaymentLink
        {
            BillingType = BillingType.BOLETO,
            ChargeType = ChargeType.DETACHED
        };

        var json = JsonSerializer.Serialize(model, options);

        Assert.Contains("\"billingType\":\"BOLETO\"", json);
        Assert.Contains("\"chargeType\":\"DETACHED\"", json);
    }

    [Fact]
    public void Deserialization_EnumDeserializesFromString()
    {
        var options = CreateTestOptions();
        var json = "{\"billingType\":\"CREDIT_CARD\",\"chargeType\":\"INSTALLMENT\"}";

        var model = JsonSerializer.Deserialize<PaymentLink>(json, options);

        Assert.NotNull(model);
        Assert.Equal(BillingType.CREDIT_CARD, model.BillingType);
        Assert.Equal(ChargeType.INSTALLMENT, model.ChargeType);
    }

    [Fact]
    public void Serialization_AllBillingTypes_SerializeCorrectly()
    {
        var options = CreateTestOptions();
        var billingTypes = new[]
        {
            BillingType.UNDEFINED, BillingType.BOLETO, BillingType.CREDIT_CARD,
            BillingType.DEBIT_CARD, BillingType.TRANSFER, BillingType.DEPOSIT, BillingType.PIX
        };

        foreach (var billingType in billingTypes)
        {
            var json = JsonSerializer.Serialize(billingType, options);
            var deserialized = JsonSerializer.Deserialize<BillingType>(json, options);
            Assert.Equal(billingType, deserialized);
        }
    }

    [Fact]
    public void Serialization_PixAddressKeyType_RoundTrips()
    {
        var options = CreateTestOptions();

        foreach (PixAddressKeyType keyType in Enum.GetValues<PixAddressKeyType>())
        {
            var json = JsonSerializer.Serialize(keyType, options);
            var deserialized = JsonSerializer.Deserialize<PixAddressKeyType>(json, options);
            Assert.Equal(keyType, deserialized);
        }
    }

    [Fact]
    public void Serialization_PixTransactionStatus_RoundTrips()
    {
        var options = CreateTestOptions();

        foreach (PixTransactionStatus status in Enum.GetValues<PixTransactionStatus>())
        {
            var json = JsonSerializer.Serialize(status, options);
            var deserialized = JsonSerializer.Deserialize<PixTransactionStatus>(json, options);
            Assert.Equal(status, deserialized);
        }
    }

    #endregion

    #region Null Handling

    [Fact]
    public void Serialization_NullPropertiesAreOmitted()
    {
        var options = CreateTestOptions();
        var model = new PaymentLink
        {
            Id = "test",
            Name = null,
            EndDate = null
        };

        var json = JsonSerializer.Serialize(model, options);

        Assert.Contains("\"id\":\"test\"", json);
        Assert.DoesNotContain("\"name\":", json);
        Assert.DoesNotContain("\"endDate\":", json);
    }

    [Fact]
    public void Serialization_NonNullPropertiesAreIncluded()
    {
        var options = CreateTestOptions();
        var model = new PaymentLink
        {
            Id = "test",
            Name = "My Link",
            Value = 100m
        };

        var json = JsonSerializer.Serialize(model, options);

        Assert.Contains("\"id\":\"test\"", json);
        Assert.Contains("\"name\":\"My Link\"", json);
        Assert.Contains("\"value\":100", json);
    }

    #endregion

    #region Case Insensitive Deserialization

    [Fact]
    public void Deserialization_CaseInsensitive()
    {
        var options = CreateTestOptions();

        // Using PascalCase property names in JSON
        var json = "{\"Id\":\"test123\",\"Name\":\"Test\",\"Value\":50}";

        var model = JsonSerializer.Deserialize<PaymentLink>(json, options);

        Assert.NotNull(model);
        Assert.Equal("test123", model.Id);
        Assert.Equal("Test", model.Name);
        Assert.Equal(50m, model.Value);
    }

    [Fact]
    public void Deserialization_CamelCaseInput()
    {
        var options = CreateTestOptions();

        var json = "{\"id\":\"test456\",\"name\":\"Camel Test\",\"value\":75}";

        var model = JsonSerializer.Deserialize<PaymentLink>(json, options);

        Assert.NotNull(model);
        Assert.Equal("test456", model.Id);
        Assert.Equal("Camel Test", model.Name);
    }

    #endregion

    #region Round Trip

    [Fact]
    public void Serialization_RoundTrip_PreservesData()
    {
        var options = CreateTestOptions();
        var original = new PaymentLink
        {
            Id = "pl_roundtrip",
            Name = "Round Trip Test",
            Description = "A description",
            Value = 199.99m,
            Active = true,
            BillingType = BillingType.PIX,
            ChargeType = ChargeType.RECURRENT,
            DueDateLimitDays = 30,
            MaxInstallmentCount = 12,
            NotificationEnabled = true,
            Deleted = false
        };

        var json = JsonSerializer.Serialize(original, options);
        var deserialized = JsonSerializer.Deserialize<PaymentLink>(json, options);

        Assert.NotNull(deserialized);
        Assert.Equal(original.Id, deserialized.Id);
        Assert.Equal(original.Name, deserialized.Name);
        Assert.Equal(original.Description, deserialized.Description);
        Assert.Equal(original.Value, deserialized.Value);
        Assert.Equal(original.Active, deserialized.Active);
        Assert.Equal(original.BillingType, deserialized.BillingType);
        Assert.Equal(original.ChargeType, deserialized.ChargeType);
        Assert.Equal(original.DueDateLimitDays, deserialized.DueDateLimitDays);
        Assert.Equal(original.MaxInstallmentCount, deserialized.MaxInstallmentCount);
        Assert.Equal(original.NotificationEnabled, deserialized.NotificationEnabled);
        Assert.Equal(original.Deleted, deserialized.Deleted);
    }

    #endregion
}
