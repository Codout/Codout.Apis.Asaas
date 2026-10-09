using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Codout.Apis.Asaas.Models.Payment;

namespace Codout.Apis.Asaas.Core;

/// <summary>Accepts the current payment object and the legacy payment ID string.</summary>
internal sealed class PaymentSplitPaymentInfoConverter : JsonConverter<PaymentSplitPaymentInfo>
{
    public override PaymentSplitPaymentInfo Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => new PaymentSplitPaymentInfo { Id = reader.GetString() },
            // This converter is attached to the property, not the type, so the
            // nested call uses the normal object converter and the SDK date converters.
            JsonTokenType.StartObject => JsonSerializer.Deserialize<PaymentSplitPaymentInfo>(ref reader, options),
            _ => throw new JsonException(
                $"Invalid payment token {reader.TokenType}: expected an object, a string ID, or null.")
        };
    }

    public override void Write(Utf8JsonWriter writer, PaymentSplitPaymentInfo value,
        JsonSerializerOptions options)
    {
        // Preserve legacy string serialization for references containing only an ID.
        if (value.Id is not null && value.ConfirmedDate is null &&
            value.InvoiceNumber is null && value.ExternalReference is null)
            writer.WriteStringValue(value.Id);
        else
            JsonSerializer.Serialize(writer, value, options);
    }
}
