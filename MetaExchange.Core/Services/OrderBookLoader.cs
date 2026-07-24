using System.Text.Json;
using MetaExchange.Dtos;
using MetaExchange.Models;

namespace MetaExchange.Services;

/// <inheritdoc />
public sealed class OrderBookLoader : IOrderBookLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <inheritdoc />
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
}
