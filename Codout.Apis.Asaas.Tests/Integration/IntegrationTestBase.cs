using System;
using Codout.Apis.Asaas.Core;

namespace Codout.Apis.Asaas.Tests.Integration;

/// <summary>
/// Base para testes de integracao reais contra api-sandbox.asaas.com.
/// Constroi AsaasApi de verdade (sem mocks) usando o token de sandbox da env var.
/// </summary>
[Trait("Category", "Integration")]
public abstract class IntegrationTestBase
{
    protected AsaasApi Asaas { get; }

    protected IntegrationTestBase()
    {
        var token = Environment.GetEnvironmentVariable(IntegrationFactAttribute.EnvVarName)
            ?? throw new InvalidOperationException(
                $"{IntegrationFactAttribute.EnvVarName} nao definida. " +
                "IntegrationFact deveria ter dado skip antes de chegar aqui.");

        var settings = new ApiSettings(token, "AsaasSdkContractTests", AsaasEnvironment.SANDBOX);
        Asaas = new AsaasApi(settings);
    }
}
