namespace Codout.Apis.Asaas.Models.PaymentDunning.Enums;

public enum PaymentDunningType
{
    CREDIT_BUREAU,
    // DEBT_RECOVERY_ASSISTANCE so e aceito pelo FILTER do List endpoint
    // (PaymentDunningListRequestPaymentDunningType). Save/response retornam
    // sempre CREDIT_BUREAU. Mantido aqui para que o filter compile.
    DEBT_RECOVERY_ASSISTANCE
}

public static class PaymentDunningTypeExtension
{
    public static bool IsCreditBureau(this PaymentDunningType type)
    {
        return type == PaymentDunningType.CREDIT_BUREAU;
    }
}
