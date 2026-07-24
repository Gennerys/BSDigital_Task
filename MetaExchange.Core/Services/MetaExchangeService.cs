using MetaExchange.Models;

namespace MetaExchange.Services;

/// <inheritdoc />
public sealed class MetaExchangeService : IMetaExchangeService
{
    /// <inheritdoc />
    public ExecutionPlan GetBestExecution(
        IReadOnlyList<Exchange> exchanges,
        OrderType orderType,
        decimal amountBtc)
    {
        if (amountBtc <= 0)
            throw new ArgumentOutOfRangeException(nameof(amountBtc), "Amount must be positive.");

        if (exchanges.Count == 0)
            throw new ArgumentException("At least one exchange is required.", nameof(exchanges));

        var remainingEur = exchanges.ToDictionary(e => e.Id, e => e.EurBalance);
        var remainingBtc = exchanges.ToDictionary(e => e.Id, e => e.BtcBalance);

        var levels = CollectLevels(exchanges, orderType);
        var orders = new List<PlannedOrder>();
        var remainingBtcToFill = amountBtc;

        foreach (var level in levels)
        {
            if (remainingBtcToFill <= 0)
                break;

            decimal affordableBtc;
            if (orderType == OrderType.Buy)
            {
                var eurLeft = remainingEur[level.ExchangeId];
                affordableBtc = level.PriceEur > 0 ? eurLeft / level.PriceEur : 0m;
            }
            else
            {
                affordableBtc = remainingBtc[level.ExchangeId];
            }

            var takeBtc = Min(remainingBtcToFill, level.AmountBtc, affordableBtc);
            if (takeBtc <= 0)
                continue;

            orders.Add(new PlannedOrder
            {
                ExchangeId = level.ExchangeId,
                OrderType = orderType,
                AmountBtc = takeBtc,
                PriceEur = level.PriceEur
            });

            remainingBtcToFill -= takeBtc;

            if (orderType == OrderType.Buy)
                remainingEur[level.ExchangeId] -= takeBtc * level.PriceEur;
            else
                remainingBtc[level.ExchangeId] -= takeBtc;
        }

        return new ExecutionPlan
        {
            OrderType = orderType,
            RequestedAmountBtc = amountBtc,
            Orders = orders
        };
    }

    private static List<BookLevel> CollectLevels(
        IReadOnlyList<Exchange> exchanges,
        OrderType orderType)
    {
        var levels = new List<BookLevel>();

        foreach (var exchange in exchanges)
        {
            var entries = orderType == OrderType.Buy
                ? exchange.OrderBook.Asks
                : exchange.OrderBook.Bids;

            foreach (var entry in entries)
            {
                if (entry.AmountBtc <= 0 || entry.PriceEur <= 0)
                    continue;

                levels.Add(new BookLevel(exchange.Id, entry.PriceEur, entry.AmountBtc));
            }
        }

        return orderType == OrderType.Buy
            ? levels.OrderBy(l => l.PriceEur).ThenBy(l => l.ExchangeId).ToList()
            : levels.OrderByDescending(l => l.PriceEur).ThenBy(l => l.ExchangeId).ToList();
    }

    private static decimal Min(decimal a, decimal b, decimal c) =>
        Math.Min(a, Math.Min(b, c));

    /// <summary>
    /// An order-book level tagged with its exchange.
    /// </summary>
    private sealed record BookLevel(string ExchangeId, decimal PriceEur, decimal AmountBtc);
}
