using MetaExchange.Models;

namespace MetaExchange.Services;

/// <summary>
/// Provides the configured exchange market snapshot used for best-execution planning.
/// </summary>
public interface IExchangeMarketService
{
    /// <summary>
    /// Returns the loaded exchanges with their order books and balances.
    /// </summary>
    /// <returns>A copy of the current exchange list.</returns>
    IReadOnlyList<Exchange> GetExchanges();
}
