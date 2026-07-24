using System.Text.Json.Serialization;
using MetaExchange.Converters;

namespace MetaExchange.Dtos;

internal sealed class OrderDto
{
    [JsonConverter(typeof(DecimalFlexibleConverter))]
    public decimal Amount { get; set; }

    [JsonConverter(typeof(DecimalFlexibleConverter))]
    public decimal Price { get; set; }
}
