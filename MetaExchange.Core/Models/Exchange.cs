namespace MetaExchange.Models;

/// <summary>
/// A crypto exchange with balances and an order book.
/// </summary>
public sealed class Exchange
{
    public required string Id { get; init; }
    public required OrderBook OrderBook { get; init; }
    public decimal EurBalance { get; set; }
    public decimal BtcBalance { get; set; }
}
