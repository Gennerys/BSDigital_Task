using MetaExchange.Models;

namespace MetaExchange.Tests;

internal static class ExchangeFactory
{
    public static Exchange Create(
        string id,
        decimal eurBalance,
        decimal btcBalance,
        IEnumerable<(decimal PriceEur, decimal AmountBtc)>? bids = null,
        IEnumerable<(decimal PriceEur, decimal AmountBtc)>? asks = null) =>
        new()
        {
            Id = id,
            EurBalance = eurBalance,
            BtcBalance = btcBalance,
            OrderBook = new OrderBook
            {
                Bids = (bids ?? []).Select(b => new OrderBookLevel
                {
                    PriceEur = b.PriceEur,
                    AmountBtc = b.AmountBtc
                }).ToList(),
                Asks = (asks ?? []).Select(a => new OrderBookLevel
                {
                    PriceEur = a.PriceEur,
                    AmountBtc = a.AmountBtc
                }).ToList()
            }
        };
}
