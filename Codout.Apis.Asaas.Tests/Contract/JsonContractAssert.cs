using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Codout.Apis.Asaas.Core;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Helpers para asserts de contrato JSON. Valida nomes exatos de campos
/// (case-sensitive), tipos de valor, e shape de envelope. Tudo via
/// JsonDocument bruto (sem deserializar para o C# model) para que o teste
/// detecte divergencias mesmo quando o model do SDK as ignora.
/// </summary>
public static class JsonContractAssert
{
    /// <summary>
    /// Serializa o objeto usando as opcoes reais do SDK e assert que o
    /// JSON produzido contem exatamente o conjunto de chaves esperado
    /// (case-sensitive). Detecta typos como "Nossonumero" vs "nossoNumero"
    /// e regressoes onde uma propriedade some.
    /// </summary>
    public static void SerializesWithKeys(object request, params string[] expectedKeys)
    {
        var json = JsonSerializer.Serialize(request, JsonSerializerConfiguration.Options);
        using var doc = JsonDocument.Parse(json);
        var actualKeys = doc.RootElement.EnumerateObject().Select(p => p.Name).ToHashSet();

        foreach (var key in expectedKeys)
        {
            Assert.True(actualKeys.Contains(key),
                $"Esperava chave JSON '{key}' no payload serializado. JSON real: {json}");
        }
    }

    /// <summary>
    /// Asserta que uma chave NAO esta presente no JSON serializado.
    /// Util quando uma propriedade do C# nao deve aparecer no wire
    /// (ex: nao envia campo nulo).
    /// </summary>
    public static void DoesNotSerializeKey(object request, string forbiddenKey)
    {
        var json = JsonSerializer.Serialize(request, JsonSerializerConfiguration.Options);
        using var doc = JsonDocument.Parse(json);
        var actualKeys = doc.RootElement.EnumerateObject().Select(p => p.Name).ToHashSet();
        Assert.False(actualKeys.Contains(forbiddenKey),
            $"Chave '{forbiddenKey}' apareceu no payload serializado mas nao deveria. JSON: {json}");
    }

    /// <summary>
    /// Deserializa o JSON da fixture usando as opcoes reais do SDK e roda
    /// o assert para verificar que todas as propriedades esperadas foram
    /// preenchidas. Detecta regressoes onde um campo da API some/renomeia.
    /// </summary>
    public static T DeserializeFixture<T>(string fixtureJson)
    {
        return JsonSerializer.Deserialize<T>(fixtureJson, JsonSerializerConfiguration.Options)!;
    }

    /// <summary>
    /// Asserta que o JSON tem uma chave especifica no root e que ela tem
    /// um valor de tipo esperado. Util para validar envelopes minimalistas
    /// como { data: [...] } onde nao ha hasMore/totalCount.
    /// </summary>
    public static void HasRootProperty(string json, string propertyName, JsonValueKind expectedKind)
    {
        using var doc = JsonDocument.Parse(json);
        Assert.True(doc.RootElement.TryGetProperty(propertyName, out var prop),
            $"Esperava propriedade root '{propertyName}'. JSON: {json}");
        Assert.Equal(expectedKind, prop.ValueKind);
    }

    /// <summary>
    /// Asserta que o JSON serializado de um Dictionary&lt;string,string&gt;
    /// (ou RequestParameters) contem exatamente o valor esperado para uma chave.
    /// Util para detectar "True" vs "true" em query strings.
    /// </summary>
    public static void QueryParamEquals(RequestParameters parameters, string key, string expectedValue)
    {
        Assert.True(parameters.ContainsKey(key), $"Esperava chave '{key}' em RequestParameters");
        Assert.Equal(expectedValue, parameters[key]);
    }
}
