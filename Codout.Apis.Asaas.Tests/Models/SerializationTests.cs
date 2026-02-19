using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Codout.Apis.Asaas.Models.PaymentLink;
using Codout.Apis.Asaas.Models.PaymentLink.Enums;
using Codout.Apis.Asaas.Models.Common;
using Codout.Apis.Asaas.Models.Common.Enums;
using Codout.Apis.Asaas.Models.Notification;
using Codout.Apis.Asaas.Models.CreditBureauReport;
using Codout.Apis.Asaas.Models.CustomerFiscalInfo;
using Codout.Apis.Asaas.Models.Pix;
using Codout.Apis.Asaas.Models.Pix.Enums;

namespace Codout.Apis.Asaas.Tests.Serialization;

public class SerializationTests
{
    // Replicate the SDK's JSON configuration for testing
    private static readonly JsonSerializerOptions Options = CreateOptions();

    private static JsonSerializerOptions CreateOptions()
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

    #region PaymentLink Models

    [Fact]
    public void PaymentLink_Deserialize_FromApiJson()
    {
        var json = """
        {
            "id": "pl_abc123",
            "name": "Monthly Subscription",
            "description": "Access to premium features",
            "url": "https://www.asaas.com/c/abc123",
            "value": 49.90,
            "active": true,
            "billingType": "CREDIT_CARD",
            "chargeType": "RECURRENT",
            "dueDateLimitDays": 10,
            "subscriptionCycle": "MONTHLY",
            "maxInstallmentCount": 1,
            "notificationEnabled": true,
            "endDate": "2025-12-31T00:00:00",
            "deleted": false
        }
        """;

        var result = JsonSerializer.Deserialize<PaymentLink>(json, Options);

        Assert.NotNull(result);
        Assert.Equal("pl_abc123", result.Id);
        Assert.Equal("Monthly Subscription", result.Name);
        Assert.Equal("Access to premium features", result.Description);
        Assert.Equal("https://www.asaas.com/c/abc123", result.Url);
        Assert.Equal(49.90m, result.Value);
        Assert.True(result.Active);
        Assert.Equal(BillingType.CREDIT_CARD, result.BillingType);
        Assert.Equal(ChargeType.RECURRENT, result.ChargeType);
        Assert.Equal(10, result.DueDateLimitDays);
        Assert.Equal("MONTHLY", result.SubscriptionCycle);
        Assert.Equal(1, result.MaxInstallmentCount);
        Assert.True(result.NotificationEnabled);
        Assert.NotNull(result.EndDate);
        Assert.False(result.Deleted);
    }

    [Fact]
    public void CreatePaymentLinkRequest_Serialize_ProducesCamelCase()
    {
        var request = new CreatePaymentLinkRequest
        {
            Name = "Test",
            Value = 100.00m,
            BillingType = BillingType.BOLETO,
            ChargeType = ChargeType.DETACHED,
            DueDateLimitDays = 5,
            MaxInstallmentCount = 1,
            NotificationEnabled = true
        };

        var json = JsonSerializer.Serialize(request, Options);

        Assert.Contains("\"name\":\"Test\"", json);
        Assert.Contains("\"value\":100", json);
        Assert.Contains("\"billingType\":\"BOLETO\"", json);
        Assert.Contains("\"chargeType\":\"DETACHED\"", json);
        Assert.Contains("\"dueDateLimitDays\":5", json);
        Assert.Contains("\"maxInstallmentCount\":1", json);
        Assert.Contains("\"notificationEnabled\":true", json);
    }

    [Fact]
    public void UpdatePaymentLinkRequest_Serialize_OmitsNullFields()
    {
        var request = new UpdatePaymentLinkRequest
        {
            Name = "Updated",
            Active = true
            // All other nullable fields remain null
        };

        var json = JsonSerializer.Serialize(request, Options);

        Assert.Contains("\"name\":\"Updated\"", json);
        Assert.Contains("\"active\":true", json);
        Assert.DoesNotContain("\"endDate\":", json);
        Assert.DoesNotContain("\"value\":", json);
        Assert.DoesNotContain("\"billingType\":", json);
        Assert.DoesNotContain("\"chargeType\":", json);
        Assert.DoesNotContain("\"dueDateLimitDays\":", json);
        Assert.DoesNotContain("\"maxInstallmentCount\":", json);
        Assert.DoesNotContain("\"notificationEnabled\":", json);
    }

    [Fact]
    public void PaymentLinkImage_RoundTrip()
    {
        var original = new PaymentLinkImage
        {
            Id = "img_001",
            Main = true,
            PaymentLink = "pl_123"
        };

        var json = JsonSerializer.Serialize(original, Options);
        var deserialized = JsonSerializer.Deserialize<PaymentLinkImage>(json, Options);

        Assert.NotNull(deserialized);
        Assert.Equal(original.Id, deserialized.Id);
        Assert.Equal(original.Main, deserialized.Main);
        Assert.Equal(original.PaymentLink, deserialized.PaymentLink);
    }

    #endregion

    #region Notification Models

    [Fact]
    public void Notification_Deserialize_AllFields()
    {
        var json = """
        {
            "id": "not_123",
            "customer": "cus_abc",
            "enabled": true,
            "emailEnabledForProvider": true,
            "smsEnabledForProvider": false,
            "emailEnabledForCustomer": true,
            "smsEnabledForCustomer": false,
            "phoneCallEnabledForCustomer": false,
            "whatsappEnabledForCustomer": true,
            "scheduleOffset": 3
        }
        """;

        var result = JsonSerializer.Deserialize<Notification>(json, Options);

        Assert.NotNull(result);
        Assert.Equal("not_123", result.Id);
        Assert.Equal("cus_abc", result.Customer);
        Assert.True(result.Enabled);
        Assert.True(result.EmailEnabledForProvider);
        Assert.False(result.SmsEnabledForProvider);
        Assert.True(result.EmailEnabledForCustomer);
        Assert.False(result.SmsEnabledForCustomer);
        Assert.False(result.PhoneCallEnabledForCustomer);
        Assert.True(result.WhatsappEnabledForCustomer);
        Assert.Equal(3, result.ScheduleOffset);
    }

    [Fact]
    public void UpdateNotificationRequest_Serialize()
    {
        var request = new UpdateNotificationRequest
        {
            Enabled = true,
            EmailEnabledForProvider = false,
            SmsEnabledForProvider = true,
            EmailEnabledForCustomer = true,
            SmsEnabledForCustomer = false,
            PhoneCallEnabledForCustomer = false,
            ScheduleOffset = 7
        };

        var json = JsonSerializer.Serialize(request, Options);

        Assert.Contains("\"enabled\":true", json);
        Assert.Contains("\"emailEnabledForProvider\":false", json);
        Assert.Contains("\"smsEnabledForProvider\":true", json);
        Assert.Contains("\"scheduleOffset\":7", json);
    }

    [Fact]
    public void BatchUpdateNotificationRequest_Serialize()
    {
        var request = new BatchUpdateNotificationRequest
        {
            Customer = "cus_abc",
            Notifications = new List<NotificationItem>
            {
                new NotificationItem
                {
                    Id = "not_1",
                    Enabled = true,
                    WhatsappEnabledForCustomer = true
                }
            }
        };

        var json = JsonSerializer.Serialize(request, Options);

        Assert.Contains("\"customer\":\"cus_abc\"", json);
        Assert.Contains("\"notifications\":[", json);
        Assert.Contains("\"id\":\"not_1\"", json);
        Assert.Contains("\"whatsappEnabledForCustomer\":true", json);
    }

    [Fact]
    public void BatchUpdateNotificationResponse_Deserialize()
    {
        var json = """
        {
            "notifications": [
                {"id": "not_1", "customer": "cus_abc", "enabled": true, "emailEnabledForProvider": false, "smsEnabledForProvider": false, "emailEnabledForCustomer": false, "smsEnabledForCustomer": false, "phoneCallEnabledForCustomer": false, "whatsappEnabledForCustomer": false},
                {"id": "not_2", "customer": "cus_abc", "enabled": false, "emailEnabledForProvider": false, "smsEnabledForProvider": false, "emailEnabledForCustomer": false, "smsEnabledForCustomer": false, "phoneCallEnabledForCustomer": false, "whatsappEnabledForCustomer": false}
            ]
        }
        """;

        var result = JsonSerializer.Deserialize<BatchUpdateNotificationResponse>(json, Options);

        Assert.NotNull(result);
        Assert.NotNull(result.Notifications);
        Assert.Equal(2, result.Notifications.Count);
        Assert.Equal("not_1", result.Notifications[0].Id);
        Assert.True(result.Notifications[0].Enabled);
        Assert.Equal("not_2", result.Notifications[1].Id);
        Assert.False(result.Notifications[1].Enabled);
    }

    #endregion

    #region CreditBureauReport Models

    [Fact]
    public void CreditBureauReport_Deserialize()
    {
        var json = """
        {
            "id": "cbr_123",
            "customer": "cus_abc",
            "cpfCnpj": "12345678901",
            "state": "SP",
            "status": "DONE",
            "dateCreated": "2024-06-15T10:30:00"
        }
        """;

        var result = JsonSerializer.Deserialize<CreditBureauReport>(json, Options);

        Assert.NotNull(result);
        Assert.Equal("cbr_123", result.Id);
        Assert.Equal("cus_abc", result.Customer);
        Assert.Equal("12345678901", result.CpfCnpj);
        Assert.Equal("SP", result.State);
        Assert.Equal("DONE", result.Status);
        Assert.Equal(new DateTime(2024, 6, 15, 10, 30, 0), result.DateCreated);
    }

    [Fact]
    public void CreateCreditBureauReportRequest_Serialize()
    {
        var request = new CreateCreditBureauReportRequest
        {
            Customer = "cus_test",
            CpfCnpj = "98765432100",
            State = "RJ"
        };

        var json = JsonSerializer.Serialize(request, Options);

        Assert.Contains("\"customer\":\"cus_test\"", json);
        Assert.Contains("\"cpfCnpj\":\"98765432100\"", json);
        Assert.Contains("\"state\":\"RJ\"", json);
    }

    #endregion

    #region CustomerFiscalInfo Models

    [Fact]
    public void CustomerFiscalInfo_Deserialize()
    {
        var json = """
        {
            "email": "fiscal@company.com",
            "municipalInscription": "12345",
            "stateInscription": "67890",
            "simplesNacional": true,
            "culturalProjectsPromoter": false,
            "cnae": "6201-5/00",
            "specialTaxRegime": "MICROEMPRESA",
            "serviceListItem": "14.01",
            "rpsSerie": "A",
            "rpsNumber": "100",
            "loteNumber": "1",
            "username": "testuser",
            "accessToken": "token123"
        }
        """;

        var result = JsonSerializer.Deserialize<CustomerFiscalInfo>(json, Options);

        Assert.NotNull(result);
        Assert.Equal("fiscal@company.com", result.Email);
        Assert.Equal("12345", result.MunicipalInscription);
        Assert.Equal("67890", result.StateInscription);
        Assert.True(result.SimplesNacional);
        Assert.False(result.CulturalProjectsPromoter);
        Assert.Equal("6201-5/00", result.Cnae);
        Assert.Equal("MICROEMPRESA", result.SpecialTaxRegime);
        Assert.Equal("14.01", result.ServiceListItem);
        Assert.Equal("A", result.RpsSerie);
        Assert.Equal("100", result.RpsNumber);
        Assert.Equal("1", result.LoteNumber);
        Assert.Equal("testuser", result.Username);
        Assert.Equal("token123", result.AccessToken);
    }

    [Fact]
    public void MunicipalOption_Deserialize()
    {
        var json = "{\"id\":\"opt_1\",\"label\":\"Sao Paulo - SP\"}";

        var result = JsonSerializer.Deserialize<MunicipalOption>(json, Options);

        Assert.NotNull(result);
        Assert.Equal("opt_1", result.Id);
        Assert.Equal("Sao Paulo - SP", result.Label);
    }

    [Fact]
    public void MunicipalOption_RoundTrip()
    {
        var original = new MunicipalOption
        {
            Id = "mo_abc",
            Label = "Rio de Janeiro - RJ"
        };

        var json = JsonSerializer.Serialize(original, Options);
        var deserialized = JsonSerializer.Deserialize<MunicipalOption>(json, Options);

        Assert.NotNull(deserialized);
        Assert.Equal(original.Id, deserialized.Id);
        Assert.Equal(original.Label, deserialized.Label);
    }

    #endregion

    #region Pix Models

    [Fact]
    public void PixTransaction_Deserialize_WithEnum()
    {
        var json = """
        {
            "id": "pix_tx_001",
            "payment": "pay_123",
            "status": "DONE",
            "value": 250.75,
            "description": "Pix payment received",
            "transactionDate": "2024-07-01T14:30:00",
            "scheduleDate": null
        }
        """;

        var result = JsonSerializer.Deserialize<PixTransaction>(json, Options);

        Assert.NotNull(result);
        Assert.Equal("pix_tx_001", result.Id);
        Assert.Equal("pay_123", result.Payment);
        Assert.Equal(PixTransactionStatus.DONE, result.Status);
        Assert.Equal(250.75m, result.Value);
        Assert.Equal("Pix payment received", result.Description);
        Assert.NotNull(result.TransactionDate);
        Assert.Null(result.ScheduleDate);
    }

    [Fact]
    public void PixTransaction_AllStatuses()
    {
        var statuses = new[] { "PENDING", "DONE", "CANCELLED", "SCHEDULED", "FAILED" };
        var expectedEnums = new[]
        {
            PixTransactionStatus.PENDING, PixTransactionStatus.DONE, PixTransactionStatus.CANCELLED,
            PixTransactionStatus.SCHEDULED, PixTransactionStatus.FAILED
        };

        for (int i = 0; i < statuses.Length; i++)
        {
            var json = $"{{\"id\":\"tx_{i}\",\"status\":\"{statuses[i]}\",\"value\":10}}";
            var result = JsonSerializer.Deserialize<PixTransaction>(json, Options);
            Assert.Equal(expectedEnums[i], result!.Status);
        }
    }

    [Fact]
    public void PixStaticQrCode_Deserialize()
    {
        var json = """
        {
            "id": "qr_001",
            "encodedImage": "iVBORw0KGgo...",
            "payload": "00020126...",
            "allowsMultiplePayments": true,
            "expirationDate": "2025-01-01T00:00:00"
        }
        """;

        var result = JsonSerializer.Deserialize<PixStaticQrCode>(json, Options);

        Assert.NotNull(result);
        Assert.Equal("qr_001", result.Id);
        Assert.Equal("iVBORw0KGgo...", result.EncodedImage);
        Assert.Equal("00020126...", result.Payload);
        Assert.True(result.AllowsMultiplePayments);
        Assert.NotNull(result.ExpirationDate);
    }

    [Fact]
    public void CreatePixStaticQrCodeRequest_Serialize()
    {
        var request = new CreatePixStaticQrCodeRequest
        {
            AddressKey = "key_abc",
            Description = "QR Code for donations",
            Value = 10.00m
        };

        var json = JsonSerializer.Serialize(request, Options);

        Assert.Contains("\"addressKey\":\"key_abc\"", json);
        Assert.Contains("\"description\":\"QR Code for donations\"", json);
        Assert.Contains("\"value\":10", json);
    }

    [Fact]
    public void DecodedPixQrCode_Deserialize()
    {
        var json = """
        {
            "payload": "00020126...",
            "type": "STATIC",
            "endToEndIdentifier": "E12345678",
            "originalValue": 55.50,
            "receiverName": "Maria Silva"
        }
        """;

        var result = JsonSerializer.Deserialize<DecodedPixQrCode>(json, Options);

        Assert.NotNull(result);
        Assert.Equal("00020126...", result.Payload);
        Assert.Equal("STATIC", result.Type);
        Assert.Equal("E12345678", result.EndToEndIdentifier);
        Assert.Equal(55.50m, result.OriginalValue);
        Assert.Equal("Maria Silva", result.ReceiverName);
    }

    [Fact]
    public void PayPixQrCodeRequest_Serialize_WithNestedObject()
    {
        var request = new PayPixQrCodeRequest
        {
            QrCode = new PayPixQrCodeInfo { Payload = "00020126..." },
            Value = 100.00m,
            Description = "Payment via QR",
            ScheduleDate = new DateTime(2024, 12, 25)
        };

        var json = JsonSerializer.Serialize(request, Options);

        Assert.Contains("\"qrCode\":", json);
        Assert.Contains("\"payload\":\"00020126...\"", json);
        Assert.Contains("\"value\":100", json);
        Assert.Contains("\"description\":\"Payment via QR\"", json);
        Assert.Contains("\"scheduleDate\":", json);
    }

    [Fact]
    public void PixPayment_Deserialize()
    {
        var json = """
        {
            "id": "pix_pay_001",
            "value": 300.00,
            "description": "QR Code payment",
            "status": "PENDING",
            "scheduleDate": "2024-08-15T00:00:00",
            "transactionReceiptUrl": "https://example.com/receipt/123"
        }
        """;

        var result = JsonSerializer.Deserialize<PixPayment>(json, Options);

        Assert.NotNull(result);
        Assert.Equal("pix_pay_001", result.Id);
        Assert.Equal(300.00m, result.Value);
        Assert.Equal("QR Code payment", result.Description);
        Assert.Equal("PENDING", result.Status);
        Assert.NotNull(result.ScheduleDate);
        Assert.Equal("https://example.com/receipt/123", result.TransactionReceiptUrl);
    }

    [Fact]
    public void PixAddressKey_Deserialize_WithEnum()
    {
        var json = """
        {
            "id": "key_001",
            "key": "12345678901",
            "type": "CPF",
            "status": "ACTIVE",
            "dateCreated": "2024-03-01T10:00:00"
        }
        """;

        var result = JsonSerializer.Deserialize<PixAddressKey>(json, Options);

        Assert.NotNull(result);
        Assert.Equal("key_001", result.Id);
        Assert.Equal("12345678901", result.Key);
        Assert.Equal(PixAddressKeyType.CPF, result.Type);
        Assert.Equal("ACTIVE", result.Status);
        Assert.Equal(new DateTime(2024, 3, 1, 10, 0, 0), result.DateCreated);
    }

    [Fact]
    public void CreatePixAddressKeyRequest_Serialize_EnumAsString()
    {
        var request = new CreatePixAddressKeyRequest { Type = PixAddressKeyType.EMAIL };

        var json = JsonSerializer.Serialize(request, Options);

        Assert.Contains("\"type\":\"EMAIL\"", json);
        Assert.DoesNotContain("\"type\":3", json); // Should NOT be numeric
    }

    [Fact]
    public void PixAddressKey_AllKeyTypes_Deserialize()
    {
        var keyTypes = new[] { "CPF", "CNPJ", "EMAIL", "PHONE", "EVP" };
        var expectedEnums = new[]
        {
            PixAddressKeyType.CPF, PixAddressKeyType.CNPJ, PixAddressKeyType.EMAIL,
            PixAddressKeyType.PHONE, PixAddressKeyType.EVP
        };

        for (int i = 0; i < keyTypes.Length; i++)
        {
            var json = $"{{\"id\":\"k_{i}\",\"key\":\"value\",\"type\":\"{keyTypes[i]}\",\"status\":\"ACTIVE\",\"dateCreated\":\"2024-01-01T00:00:00\"}}";
            var result = JsonSerializer.Deserialize<PixAddressKey>(json, Options);
            Assert.Equal(expectedEnums[i], result!.Type);
        }
    }

    #endregion

    #region Common Models

    [Fact]
    public void Discount_Serialize()
    {
        var discount = new Discount
        {
            Value = 10.0m,
            DueDateLimitDays = 5,
            Type = DiscountType.PERCENTAGE
        };

        var json = JsonSerializer.Serialize(discount, Options);

        Assert.Contains("\"value\":10", json);
        Assert.Contains("\"dueDateLimitDays\":5", json);
        Assert.Contains("\"type\":\"PERCENTAGE\"", json);
    }

    [Fact]
    public void Discount_Deserialize()
    {
        var json = "{\"value\":15.5,\"dueDateLimitDays\":3,\"type\":\"FIXED\"}";

        var result = JsonSerializer.Deserialize<Discount>(json, Options);

        Assert.NotNull(result);
        Assert.Equal(15.5m, result.Value);
        Assert.Equal(3, result.DueDateLimitDays);
        Assert.Equal(DiscountType.FIXED, result.Type);
    }

    [Fact]
    public void Fine_RoundTrip()
    {
        var original = new Fine { Value = 2.5m };

        var json = JsonSerializer.Serialize(original, Options);
        var deserialized = JsonSerializer.Deserialize<Fine>(json, Options);

        Assert.NotNull(deserialized);
        Assert.Equal(original.Value, deserialized.Value);
    }

    [Fact]
    public void Interest_RoundTrip()
    {
        var original = new Interest { Value = 1.5m };

        var json = JsonSerializer.Serialize(original, Options);
        var deserialized = JsonSerializer.Deserialize<Interest>(json, Options);

        Assert.NotNull(deserialized);
        Assert.Equal(original.Value, deserialized.Value);
    }

    #endregion

    #region Error Response Model

    [Fact]
    public void Error_Deserialize_WithJsonPropertyName()
    {
        var json = "{\"code\":\"invalid_value\",\"description\":\"The value must be positive\"}";

        var result = JsonSerializer.Deserialize<Codout.Apis.Asaas.Core.Response.Error>(json, Options);

        Assert.NotNull(result);
        Assert.Equal("invalid_value", result.Code);
        Assert.Equal("The value must be positive", result.Description);
    }

    [Fact]
    public void ErrorList_Deserialize()
    {
        var json = "[{\"code\":\"err1\",\"description\":\"First\"},{\"code\":\"err2\",\"description\":\"Second\"}]";

        var result = JsonSerializer.Deserialize<List<Codout.Apis.Asaas.Core.Response.Error>>(json, Options);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("err1", result[0].Code);
        Assert.Equal("err2", result[1].Code);
    }

    #endregion
}
