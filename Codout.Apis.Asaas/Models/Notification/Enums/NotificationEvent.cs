namespace Codout.Apis.Asaas.Models.Notification.Enums;

/// <summary>
/// Eventos que disparam notificacao para o cliente. Schema oficial:
/// NotificationGetResponseNotificationEvent enum (6 valores).
/// </summary>
public enum NotificationEvent
{
    PAYMENT_CREATED,
    PAYMENT_UPDATED,
    PAYMENT_RECEIVED,
    PAYMENT_OVERDUE,
    PAYMENT_DUEDATE_WARNING,
    SEND_LINHA_DIGITAVEL
}
