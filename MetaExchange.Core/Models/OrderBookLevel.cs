namespace MetaExchange.Models;

/// <summary>
/// A single price and amount level in an order book.
/// </summary>
public sealed class OrderBookLevel
{
    public decimal PriceEur { get; init; }
    public decimal AmountBtc { get; init; }
}
