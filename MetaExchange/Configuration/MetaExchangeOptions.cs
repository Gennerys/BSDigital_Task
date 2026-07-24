namespace MetaExchange.Configuration;

public sealed class MetaExchangeOptions
{
    public const string SectionName = "MetaExchange";

    public string OrderBooksFilePath { get; set; } = "order_books_data";

    public int ExchangeCount { get; set; } = 3;

    public decimal DefaultEurBalance { get; set; } = 100_000m;

    public decimal DefaultBtcBalance { get; set; } = 10m;
}
