using MetaExchange.Models;

namespace MetaExchange.Services;

/// <summary>
/// Loads exchange order books from a data file.
/// </summary>
public interface IOrderBookLoader
{
    /// <summary>
    /// Reads the first <paramref name="count"/> order books from <paramref name="filePath"/>
    /// and maps them to exchanges with balances provided by <paramref name="balanceFactory"/>.
    /// </summary>
    /// <param name="filePath">Path to the order-books file (one JSON order book per line).</param>
    /// <param name="count">Number of exchanges to load; must be positive.</param>
    /// <param name="balanceFactory">
    /// Factory that returns EUR and BTC balances for a given exchange id.
    /// </param>
    /// <returns>The loaded exchanges with normalized order books and balances.</returns>
    IReadOnlyList<Exchange> LoadExchanges(
        string filePath,
        int count,
        Func<string, (decimal EurBalance, decimal BtcBalance)> balanceFactory);
}
