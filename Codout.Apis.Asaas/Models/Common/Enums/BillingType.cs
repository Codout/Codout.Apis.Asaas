namespace Codout.Apis.Asaas.Models.Common.Enums;

public enum BillingType
{
    UNDEFINED, BOLETO, CREDIT_CARD, DEBIT_CARD, TRANSFER, DEPOSIT, PIX
}

public static class BillingTypeExtension
{
    public static bool IsBoleto(this BillingType billingType)
    {
        return billingType == BillingType.BOLETO;
    }

    public static bool IsCreditCard(this BillingType billingType)
    {
        return billingType == BillingType.CREDIT_CARD;
    }

    public static bool IsUndefined(this BillingType billingType)
    {
        return billingType == BillingType.UNDEFINED;
    }

    public static bool IsDebitCard(this BillingType billingType)
    {
        return billingType == BillingType.DEBIT_CARD;
    }

    public static bool IsTransfer(this BillingType billingType)
    {
        return billingType == BillingType.TRANSFER;
    }

    public static bool IsDeposit(this BillingType billingType)
    {
        return billingType == BillingType.DEPOSIT;
    }

    public static bool IsPIX(this BillingType billingType)
    {
        return billingType == BillingType.PIX;
    }
}
