namespace Codout.Apis.Asaas.Models.Transfer.Enums;

/// <summary>
/// Schema oficial unifica todos os tipos de transfer no mesmo enum
/// (PENDING, BANK_PROCESSING, DONE, CANCELLED, FAILED). Antes faltavam
/// BANK_PROCESSING e FAILED em AsaasAccountTransferStatus.
/// </summary>
public enum AsaasAccountTransferStatus
{
    PENDING,
    BANK_PROCESSING,
    DONE,
    CANCELLED,
    FAILED
}
