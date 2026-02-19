using System.Text.Json;
using System.Text.Json.Serialization;

namespace Codout.Apis.Asaas.Core;

internal static class JsonSerializerConfiguration
{
    internal static JsonSerializerOptions Options { get; } = CreateOptions();

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        options.Converters.Add(new SafeEnumConverterFactory());
        options.Converters.Add(new FlexibleDateTimeConverter());
        options.Converters.Add(new FlexibleNullableDateTimeConverter());

        return options;
    }
}
