namespace Codout.Apis.Asaas.Models.Transfer.Enums;

/// <summary>
/// Modalidade da transferencia retornada pelo campo operationType do schema:
/// PIX (chave Pix), TED (transferencia para banco diferente), INTERNAL (entre
/// contas Asaas).
/// </summary>
public enum TransferOperationType
{
    PIX,
    TED,
    INTERNAL
}
