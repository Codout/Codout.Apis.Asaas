using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Managers;
using Codout.Apis.Asaas.Models.Notification;
using Codout.Apis.Asaas.Tests.Helpers;

namespace Codout.Apis.Asaas.Tests.Managers;

public class NotificationManagerTests : ManagerTestBase<NotificationManager>
{
    protected override NotificationManager CreateManager(ApiSettings settings, MockHttpMessageHandler handler)
        => new TestableNotificationManager(settings, handler);

    #region Update

    [Fact]
    public async Task Update_SendsPostRequest()
    {
        SetupOkResponse("{\"id\":\"not_123\",\"customer\":\"cus_abc\",\"enabled\":true,\"emailEnabledForProvider\":true,\"smsEnabledForProvider\":false,\"emailEnabledForCustomer\":true,\"smsEnabledForCustomer\":false,\"phoneCallEnabledForCustomer\":false,\"whatsappEnabledForCustomer\":true,\"scheduleOffset\":5}");

        var request = new UpdateNotificationRequest
        {
            Enabled = true,
            EmailEnabledForProvider = true,
            SmsEnabledForProvider = false,
            EmailEnabledForCustomer = true,
            SmsEnabledForCustomer = false,
            PhoneCallEnabledForCustomer = false,
            ScheduleOffset = 5
        };

        var result = await Manager.Update("not_123", request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/notifications/not_123");
        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.Equal("not_123", result.Data.Id);
        Assert.Equal("cus_abc", result.Data.Customer);
        Assert.True(result.Data.Enabled);
        Assert.True(result.Data.EmailEnabledForProvider);
        Assert.False(result.Data.SmsEnabledForProvider);
        Assert.True(result.Data.EmailEnabledForCustomer);
        Assert.False(result.Data.SmsEnabledForCustomer);
        Assert.False(result.Data.PhoneCallEnabledForCustomer);
        Assert.True(result.Data.WhatsappEnabledForCustomer);
        Assert.Equal(5, result.Data.ScheduleOffset);
    }

    [Fact]
    public async Task Update_SerializesRequestBody()
    {
        SetupOkResponse("{\"id\":\"not_123\"}");

        var request = new UpdateNotificationRequest
        {
            Enabled = false,
            EmailEnabledForProvider = true,
            ScheduleOffset = 10
        };

        await Manager.Update("not_123", request);

        Assert.NotNull(Handler.LastRequestContent);
        Assert.Contains("\"enabled\":false", Handler.LastRequestContent);
        Assert.Contains("\"emailEnabledForProvider\":true", Handler.LastRequestContent);
        Assert.Contains("\"scheduleOffset\":10", Handler.LastRequestContent);
    }

    #endregion

    #region BatchUpdate

    [Fact]
    public async Task BatchUpdate_SendsPostRequest()
    {
        SetupOkResponse("{\"notifications\":[{\"id\":\"not_1\",\"customer\":\"cus_abc\",\"enabled\":true,\"emailEnabledForProvider\":true,\"smsEnabledForProvider\":false,\"emailEnabledForCustomer\":false,\"smsEnabledForCustomer\":false,\"phoneCallEnabledForCustomer\":false,\"whatsappEnabledForCustomer\":false},{\"id\":\"not_2\",\"customer\":\"cus_abc\",\"enabled\":false,\"emailEnabledForProvider\":false,\"smsEnabledForProvider\":false,\"emailEnabledForCustomer\":false,\"smsEnabledForCustomer\":false,\"phoneCallEnabledForCustomer\":false,\"whatsappEnabledForCustomer\":false}]}");

        var request = new BatchUpdateNotificationRequest
        {
            Customer = "cus_abc",
            Notifications = new List<NotificationItem>
            {
                new NotificationItem
                {
                    Id = "not_1",
                    Enabled = true,
                    EmailEnabledForProvider = true
                },
                new NotificationItem
                {
                    Id = "not_2",
                    Enabled = false
                }
            }
        };

        var result = await Manager.BatchUpdate(request);

        AssertRequestMethod(HttpMethod.Post);
        AssertRequestUrl("/v3/notifications/batch");
        Assert.True(result.WasSucessfull());
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Notifications);
        Assert.Equal(2, result.Data.Notifications.Count);
        Assert.Equal("not_1", result.Data.Notifications[0].Id);
        Assert.True(result.Data.Notifications[0].Enabled);
        Assert.Equal("not_2", result.Data.Notifications[1].Id);
        Assert.False(result.Data.Notifications[1].Enabled);
    }

    [Fact]
    public async Task BatchUpdate_SerializesCustomerAndNotifications()
    {
        SetupOkResponse("{\"notifications\":[]}");

        var request = new BatchUpdateNotificationRequest
        {
            Customer = "cus_xyz",
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

        await Manager.BatchUpdate(request);

        Assert.NotNull(Handler.LastRequestContent);
        Assert.Contains("\"customer\":\"cus_xyz\"", Handler.LastRequestContent);
        Assert.Contains("\"notifications\":", Handler.LastRequestContent);
    }

    #endregion

    #region Error Handling

    [Fact]
    public async Task Update_ReturnsErrorOnBadRequest()
    {
        SetupErrorResponse(HttpStatusCode.BadRequest);

        var request = new UpdateNotificationRequest { Enabled = true };
        var result = await Manager.Update("invalid_id", request);

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task BatchUpdate_ReturnsErrorOnInternalServerError()
    {
        Handler.WithResponse(HttpStatusCode.InternalServerError, "{\"errors\":[{\"code\":\"server_error\",\"description\":\"Internal error\"}]}");

        var request = new BatchUpdateNotificationRequest
        {
            Customer = "cus_abc",
            Notifications = new List<NotificationItem>()
        };

        var result = await Manager.BatchUpdate(request);

        Assert.False(result.WasSucessfull());
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
    }

    #endregion
}
