using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using MetaExchange.Models;

namespace MetaExchange.Services;

public sealed class OrderBookLoader : IOrderBookLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IReadOnlyList<Exchange> LoadExchanges(
        string filePath,
        int count,
        Func<string, (decimal EurBalance, decimal BtcBalance)> balanceFactory)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        var exchanges = new List<Exchange>(count);

        using var reader = new StreamReader(filePath);
        for (var i = 0; i < count; i++)
        {
            var line = reader.ReadLine();
            if (line is null)
                throw new InvalidOperationException($"Expected {count} order books but file ended after {i}.");

            var tab = line.IndexOf('\t');
            var json = tab >= 0 ? line[(tab + 1)..] : line;
            var dto = JsonSerializer.Deserialize<OrderBookDto>(json, JsonOptions)
                ?? throw new InvalidOperationException($"Failed to parse order book on line {i + 1}.");

            var id = $"Exchange-{i + 1}";
            var (eurBalance, btcBalance) = balanceFactory(id);
            exchanges.Add(new Exchange
            {
                Id = id,
                OrderBook = ToOrderBook(dto),
                EurBalance = eurBalance,
                BtcBalance = btcBalance
            });
        }

        return exchanges;
    }

    private static OrderBook ToOrderBook(OrderBookDto dto) => new()
    {
        Bids = dto.Bids
            .Select(b => new OrderBookLevel
            {
                PriceEur = b.Order.Price,
                AmountBtc = b.Order.Amount
            })
            .Where(e => e.AmountBtc > 0 && e.PriceEur > 0)
            .OrderByDescending(e => e.PriceEur)
            .ToList(),
        Asks = dto.Asks
            .Select(a => new OrderBookLevel
            {
                PriceEur = a.Order.Price,
                AmountBtc = a.Order.Amount
            })
            .Where(e => e.AmountBtc > 0 && e.PriceEur > 0)
            .OrderBy(e => e.PriceEur)
            .ToList()
    };

    private sealed class OrderBookDto
    {
        public List<LevelDto> Bids { get; set; } = [];
        public List<LevelDto> Asks { get; set; } = [];
    }

    private sealed class LevelDto
    {
        public OrderDto Order { get; set; } = new();
    }

    private sealed class OrderDto
    {
        [JsonConverter(typeof(DecimalFlexibleConverter))]
        public decimal Amount { get; set; }

        [JsonConverter(typeof(DecimalFlexibleConverter))]
        public decimal Price { get; set; }
    }

    private sealed class DecimalFlexibleConverter : JsonConverter<decimal>
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
}
