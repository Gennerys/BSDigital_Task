using MetaExchange.Configuration;
using MetaExchange.Models;
using Microsoft.Extensions.Options;

namespace MetaExchange.Services;

/// <inheritdoc />
public sealed class ExchangeMarketService : IExchangeMarketService
{
    private readonly IReadOnlyList<Exchange> _exchanges;

    public ExchangeMarketService(
        IOrderBookLoader orderBookLoader,
        IOptions<MetaExchangeOptions> options,
        IHostEnvironment environment,
        ILogger<ExchangeMarketService> logger)
    {
        var settings = options.Value;
        var path = Path.IsPathRooted(settings.OrderBooksFilePath)
            ? settings.OrderBooksFilePath
            : Path.Combine(environment.ContentRootPath, settings.OrderBooksFilePath);

        if (!File.Exists(path))
            throw new FileNotFoundException($"Order books file not found: {path}");

        logger.LogInformation(
            "Loading {Count} exchange(s) from {Path}",
            settings.ExchangeCount,
            path);

        _exchanges = orderBookLoader.LoadExchanges(
            path,
            settings.ExchangeCount,
            _ => (settings.DefaultEurBalance, settings.DefaultBtcBalance));

        logger.LogInformation("Loaded {Count} exchange(s)", _exchanges.Count);
    }

    /// <inheritdoc />
    public IReadOnlyList<Exchange> GetExchanges() =>
        _exchanges
            .Select(e => new Exchange
            {
                Id = e.Id,
                OrderBook = e.OrderBook,
                EurBalance = e.EurBalance,
                BtcBalance = e.BtcBalance
            })
            .ToList();
}
