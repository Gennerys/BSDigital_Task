using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MetaExchange.Converters;

internal sealed class DecimalFlexibleConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number && reader.TryGetDecimal(out var value))
            return value;

        if (reader.TokenType == JsonTokenType.String &&
            decimal.TryParse(reader.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            return value;

        throw new JsonException($"Unexpected token for decimal: {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options) =>
        writer.WriteNumberValue(value);
}
