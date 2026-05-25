using Codout.Apis.Asaas.Models.Notification;
using Codout.Apis.Asaas.Models.Notification.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para NotificationManager + Notification model (B-34).
/// Schemas verificados via MCP em 2026-05-24.
/// </summary>
public class NotificationContractTests
{
    [Fact]
    public void NotificationResponse_DeserializesAllFields()
    {
        var json = "{\"object\":\"notification\",\"id\":\"not_wuGp97JeCr7G\",\"customer\":\"cus_000005401844\",\"enabled\":true,\"emailEnabledForProvider\":true,\"smsEnabledForProvider\":true,\"emailEnabledForCustomer\":true,\"smsEnabledForCustomer\":true,\"phoneCallEnabledForCustomer\":false,\"whatsappEnabledForCustomer\":false,\"event\":\"PAYMENT_CREATED\",\"scheduleOffset\":1,\"deleted\":false}";

        var result = JsonContractAssert.DeserializeFixture<Notification>(json);

        Assert.Equal("notification", result.Object);
        Assert.Equal("not_wuGp97JeCr7G", result.Id);
        Assert.Equal("cus_000005401844", result.Customer);
        Assert.True(result.Enabled);
        Assert.True(result.EmailEnabledForProvider);
        Assert.False(result.PhoneCallEnabledForCustomer);
        Assert.False(result.WhatsappEnabledForCustomer);
        Assert.Equal(NotificationEvent.PAYMENT_CREATED, result.Event);
        Assert.Equal(1, result.ScheduleOffset);
        Assert.False(result.Deleted);
    }

    [Fact]
    public void NotificationEvent_AllSixValuesDeserialize()
    {
        // B-34a: campo event nao existia no model.
        // Schema: PAYMENT_CREATED, PAYMENT_UPDATED, PAYMENT_RECEIVED,
        // PAYMENT_OVERDUE, PAYMENT_DUEDATE_WARNING, SEND_LINHA_DIGITAVEL
        foreach (var ev in new[] {
            "PAYMENT_CREATED", "PAYMENT_UPDATED", "PAYMENT_RECEIVED",
            "PAYMENT_OVERDUE", "PAYMENT_DUEDATE_WARNING", "SEND_LINHA_DIGITAVEL" })
        {
            var json = $"{{\"id\":\"x\",\"event\":\"{ev}\"}}";
            var result = JsonContractAssert.DeserializeFixture<Notification>(json);
            Assert.NotNull(result.Event);
            Assert.Equal(ev, result.Event.ToString());
        }
    }

    [Fact]
    public void UpdateRequest_BoolsAreNullableNotForcingFalse()
    {
        // B-34b: bool non-nullable forcava false em todo update parcial.
        var request = new UpdateNotificationRequest { Enabled = true };

        JsonContractAssert.SerializesWithKeys(request, "enabled");
        // Outros bools nao setados nao devem aparecer no JSON
        JsonContractAssert.DoesNotSerializeKey(request, "emailEnabledForProvider");
        JsonContractAssert.DoesNotSerializeKey(request, "smsEnabledForProvider");
        JsonContractAssert.DoesNotSerializeKey(request, "phoneCallEnabledForCustomer");
    }
}
