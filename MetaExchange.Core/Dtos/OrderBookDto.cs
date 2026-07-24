namespace MetaExchange.Dtos;

/// <summary>
/// JSON order-book payload deserialized from the data file.
/// </summary>
internal sealed class OrderBookDto
{
    public List<LevelDto> Bids { get; set; } = [];
    public List<LevelDto> Asks { get; set; } = [];
}
