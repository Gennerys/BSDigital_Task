namespace MetaExchange.Models;

/// <summary>
/// Best-execution result with planned orders and fill totals.
/// </summary>
public sealed class ExecutionPlan
{
    public required OrderType OrderType { get; init; }
    public required decimal RequestedAmountBtc { get; init; }
    public required IReadOnlyList<PlannedOrder> Orders { get; init; }

    public decimal FilledAmountBtc => Orders.Sum(o => o.AmountBtc);
    public decimal TotalEur => Orders.Sum(o => o.TotalEur);
    public bool IsFullyFilled => FilledAmountBtc >= RequestedAmountBtc;
}
