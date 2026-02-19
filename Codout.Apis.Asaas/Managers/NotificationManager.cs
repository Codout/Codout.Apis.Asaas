using System.Threading.Tasks;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Core.Response;
using Codout.Apis.Asaas.Models.Notification;

namespace Codout.Apis.Asaas.Managers;

public class NotificationManager(ApiSettings settings) : BaseManager(settings)
{
    private const string NotificationsRoute = "/notifications";

    public async Task<ResponseObject<Notification>> Update(string notificationId, UpdateNotificationRequest requestObj)
    {
        var route = $"{NotificationsRoute}/{notificationId}";
        return await PostAsync<Notification>(route, requestObj);
    }

    public async Task<ResponseObject<BatchUpdateNotificationResponse>> BatchUpdate(BatchUpdateNotificationRequest requestObj)
    {
        var route = $"{NotificationsRoute}/batch";
        return await PostAsync<BatchUpdateNotificationResponse>(route, requestObj);
    }
}
