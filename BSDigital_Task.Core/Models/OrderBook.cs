namespace BSDigital_Task.Models;

public sealed class OrderBook
{
    public IReadOnlyList<OrderBookLevel> Bids { get; init; } = [];
    public IReadOnlyList<OrderBookLevel> Asks { get; init; } = [];
}
