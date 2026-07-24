namespace MetaExchange.Models;

/// <summary>
/// A planned order to execute against a specific exchange.
/// </summary>
public sealed class PlannedOrder
{
    public required string ExchangeId { get; init; }
    public required OrderType OrderType { get; init; }
    public required decimal AmountBtc { get; init; }
    public required decimal PriceEur { get; init; }

    public decimal TotalEur => AmountBtc * PriceEur;
}
