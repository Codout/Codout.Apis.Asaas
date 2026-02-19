using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Codout.Apis.Asaas.Core;

/// <summary>
/// A JSON enum converter factory that gracefully handles unknown enum string values
/// by returning the default (0) value instead of throwing a JsonException.
/// This prevents deserialization failures when the Asaas API returns new enum values
/// not yet mapped in the SDK.
/// </summary>
internal sealed class SafeEnumConverterFactory : JsonConverterFactory
{
    private static readonly JsonStringEnumConverter FallbackConverter = new();

    public override bool CanConvert(Type typeToConvert)
    {
        return FallbackConverter.CanConvert(typeToConvert);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var enumType = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;
        var converterType = typeof(SafeEnumConverter<>).MakeGenericType(enumType);

        if (Nullable.GetUnderlyingType(typeToConvert) != null)
        {
            var nullableConverterType = typeof(SafeNullableEnumConverter<>).MakeGenericType(enumType);
            return (JsonConverter)Activator.CreateInstance(nullableConverterType)!;
        }

        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}

internal sealed class SafeEnumConverter<T> : JsonConverter<T> where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (Enum.TryParse<T>(value, ignoreCase: true, out var result))
                return result;

            // Unknown enum value — return default instead of crashing
            return default;
        }

        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var intValue))
            return (T)Enum.ToObject(typeof(T), intValue);

        return default;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

internal sealed class SafeNullableEnumConverter<T> : JsonConverter<T?> where T : struct, Enum
{
    private static readonly SafeEnumConverter<T> Inner = new();

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        return Inner.Read(ref reader, typeof(T), options);
    }

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            Inner.Write(writer, value.Value, options);
    }
}
