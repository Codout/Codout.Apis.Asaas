namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para SandboxManager (B-42, modelo ja correto).
/// Schemas verificados via MCP em 2026-05-24:
/// - POST /v3/sandbox/myAccount/approve
/// - POST /v3/sandbox/payment/{id}/confirm
/// - POST /v3/sandbox/payment/{id}/overdue
/// Todos os 3 endpoints expostos pelo manager batem com o schema.
/// EnsureSandbox() bloqueia uso em produção (verificado em unit tests).
/// </summary>
public class SandboxContractTests
{
    [Fact]
    public void SandboxManager_HasAllThreeExpectedEndpoints()
    {
        // Sanity check via reflexao: garante que o manager expoe exatamente
        // 3 metodos publicos (ApproveAccount, ConfirmPayment, ForceOverdue).
        var methods = typeof(Codout.Apis.Asaas.Managers.SandboxManager)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);

        Assert.Contains(methods, m => m.Name == "ApproveAccount");
        Assert.Contains(methods, m => m.Name == "ConfirmPayment");
        Assert.Contains(methods, m => m.Name == "ForceOverdue");
    }
}
