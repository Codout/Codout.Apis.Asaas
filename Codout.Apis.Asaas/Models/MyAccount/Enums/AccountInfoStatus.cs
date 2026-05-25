namespace Codout.Apis.Asaas.Models.MyAccount.Enums;

/// <summary>
/// Status do cadastro da conta (campo "status" de AccountInfoGetResponseDTO).
/// </summary>
public enum AccountInfoStatus
{
    APPROVED,
    AWAITING_ACTION_AUTHORIZATION,
    DENIED,
    PENDING
}
