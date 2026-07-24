namespace MetaExchange.Dtos;

internal sealed class OrderBookDto
{
    public List<LevelDto> Bids { get; set; } = [];
    public List<LevelDto> Asks { get; set; } = [];
}
