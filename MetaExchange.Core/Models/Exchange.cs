namespace MetaExchange.Models;

public sealed class Exchange
{
    public required string Id { get; init; }
    public required OrderBook OrderBook { get; init; }
    public decimal EurBalance { get; set; }
    public decimal BtcBalance { get; set; }
}
