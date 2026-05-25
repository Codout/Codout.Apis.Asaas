using System;

namespace Codout.Apis.Asaas.Tests.Integration;

/// <summary>
/// Marca um teste como integration test que chama o sandbox real do Asaas.
/// Skip automatico se a variavel de ambiente ASAAS_SANDBOX_TOKEN nao
/// estiver definida — assim CI local e suites unit nao quebram, e o
/// pipeline com credenciais executa.
/// </summary>
/// <remarks>
/// Use trait "Category=Integration" para filtrar: <br/>
/// dotnet test --filter "Category=Integration"
/// </remarks>
public sealed class IntegrationFactAttribute : FactAttribute
{
    public const string EnvVarName = "ASAAS_SANDBOX_TOKEN";

    public IntegrationFactAttribute()
    {
        var token = Environment.GetEnvironmentVariable(EnvVarName);
        if (string.IsNullOrWhiteSpace(token))
        {
            Skip = $"Integration test skipped: variavel de ambiente {EnvVarName} nao definida. " +
                   "Para rodar, exporte um access_token de sandbox: $env:ASAAS_SANDBOX_TOKEN='aact_YTU0...'";
        }
    }
}
