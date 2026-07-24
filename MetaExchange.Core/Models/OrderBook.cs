namespace MetaExchange.Models;

/// <summary>
/// Bid and ask levels for a single exchange.
/// </summary>
public sealed class OrderBook
{
    public IReadOnlyList<OrderBookLevel> Bids { get; init; } = [];
    public IReadOnlyList<OrderBookLevel> Asks { get; init; } = [];
}
