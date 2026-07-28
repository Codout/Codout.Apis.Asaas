using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Codout.Apis.Asaas.Core;

/// <summary>
/// Handles non-nullable <see cref="decimal"/> fields when the Asaas API returns
/// <c>null</c> (ex.: Invoice.deductions). Null is treated as <c>0</c>.
/// Also accepts numeric strings.
/// </summary>
internal sealed class FlexibleDecimalConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return 0m;

        if (reader.TokenType == JsonTokenType.Number)
            return reader.GetDecimal();

        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value))
                return 0m;

            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed))
                return parsed;

            throw new JsonException($"Unable to convert \"{value}\" to Decimal.");
        }

        throw new JsonException($"Unexpected token {reader.TokenType} when parsing Decimal.");
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        => writer.WriteNumberValue(value);
}

/// <summary>
/// Handles nullable <see cref="decimal"/> fields, preserving <c>null</c> when
/// the API omits the value.
/// </summary>
internal sealed class FlexibleNullableDecimalConverter : JsonConverter<decimal?>
{
    private static readonly FlexibleDecimalConverter Inner = new();

    public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        return Inner.Read(ref reader, typeof(decimal), options);
    }

    public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            Inner.Write(writer, value.Value, options);
    }
}
