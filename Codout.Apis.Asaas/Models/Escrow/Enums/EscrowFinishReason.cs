namespace Codout.Apis.Asaas.Models.Escrow.Enums;

public enum EscrowFinishReason
{
    CHARGEBACK,
    EXPIRED,
    INSUFFICIENT_BALANCE,
    PAYMENT_REFUNDED,
    REQUESTED_BY_CUSTOMER,
    CUSTOMER_CONFIG_DISABLED
}
