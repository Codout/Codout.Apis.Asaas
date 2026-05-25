namespace Codout.Apis.Asaas.Models.Subscription.Enums
{
    public enum SubscriptionStatus
    {
        ACTIVE,
        EXPIRED,
        INACTIVE
    }

    public static class SubscriptionStatusExtension
    {
        public static bool IsActive(this SubscriptionStatus status)
        {
            return status == SubscriptionStatus.ACTIVE;
        }

        public static bool IsExpired(this SubscriptionStatus status)
        {
            return status == SubscriptionStatus.EXPIRED;
        }

        public static bool IsInactive(this SubscriptionStatus status)
        {
            return status == SubscriptionStatus.INACTIVE;
        }
    }
}
