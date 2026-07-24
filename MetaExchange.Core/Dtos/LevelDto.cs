namespace MetaExchange.Dtos;

/// <summary>
/// A bid or ask entry wrapping an order in the source JSON.
/// </summary>
internal sealed class LevelDto
{
    public OrderDto Order { get; set; } = new();
}
