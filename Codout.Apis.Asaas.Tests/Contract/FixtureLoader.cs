using System;
using System.IO;
using System.Reflection;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Carrega arquivos JSON do diretorio Fixtures/. As fixtures sao extraidas
/// dos exemplos oficiais da documentacao Asaas (via MCP em docs.asaas.com/mcp)
/// e funcionam como contrato congelado: se a forma do JSON da API mudar, os
/// contract tests falham e nos forcam a atualizar fixture + model juntos.
/// </summary>
public static class FixtureLoader
{
    private static readonly string FixturesRoot = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
        "Fixtures");

    /// <summary>
    /// Le uma fixture pelo caminho relativo a Tests/Fixtures/.
    /// Ex: Load("Payment/limits-response.json").
    /// </summary>
    public static string Load(string relativePath)
    {
        var fullPath = Path.Combine(FixturesRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException(
                $"Fixture nao encontrada: {relativePath}. Caminho esperado: {fullPath}. " +
                "Verifique se o arquivo existe em Tests/Fixtures/ e se esta marcado como CopyToOutputDirectory no .csproj.");
        }
        return File.ReadAllText(fullPath);
    }
}
