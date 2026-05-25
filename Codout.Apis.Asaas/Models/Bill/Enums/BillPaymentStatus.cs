namespace Codout.Apis.Asaas.Models.Bill.Enums;

public enum BillPaymentStatus
{
    PENDING,
    BANK_PROCESSING,
    PAID,
    FAILED,
    CANCELLED,
    REFUNDED,
    AWAITING_CHECKOUT_RISK_ANALYSIS_REQUEST
}

public static class BillPaymentStatusExtension
{
    public static bool IsPending(this BillPaymentStatus status) => status == BillPaymentStatus.PENDING;
    public static bool IsBankProcessing(this BillPaymentStatus status) => status == BillPaymentStatus.BANK_PROCESSING;
    public static bool IsPaid(this BillPaymentStatus status) => status == BillPaymentStatus.PAID;
    public static bool IsFailed(this BillPaymentStatus status) => status == BillPaymentStatus.FAILED;
    public static bool IsCancelled(this BillPaymentStatus status) => status == BillPaymentStatus.CANCELLED;
    public static bool IsRefunded(this BillPaymentStatus status) => status == BillPaymentStatus.REFUNDED;
}
