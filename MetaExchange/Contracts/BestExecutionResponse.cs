using MetaExchange.Models;

namespace MetaExchange.Contracts;

public sealed class BestExecutionResponse
{
    public required string OrderType { get; init; }
    public required decimal RequestedAmountBtc { get; init; }
    public required decimal FilledAmountBtc { get; init; }
    public required decimal TotalEur { get; init; }
    public required bool IsFullyFilled { get; init; }
    public required IReadOnlyList<PlannedOrderDto> Orders { get; init; }

    public static BestExecutionResponse From(ExecutionPlan plan) => new()
    {
        OrderType = plan.OrderType.ToString(),
        RequestedAmountBtc = plan.RequestedAmountBtc,
        FilledAmountBtc = plan.FilledAmountBtc,
        TotalEur = plan.TotalEur,
        IsFullyFilled = plan.IsFullyFilled,
        Orders = plan.Orders
            .Select(o => new PlannedOrderDto
            {
                ExchangeId = o.ExchangeId,
                OrderType = o.OrderType.ToString(),
                AmountBtc = o.AmountBtc,
                PriceEur = o.PriceEur,
                TotalEur = o.TotalEur
            })
            .ToList()
    };
}

public sealed class PlannedOrderDto
{
    public required string ExchangeId { get; init; }
    public required string OrderType { get; init; }
    public required decimal AmountBtc { get; init; }
    public required decimal PriceEur { get; init; }
    public required decimal TotalEur { get; init; }
}
