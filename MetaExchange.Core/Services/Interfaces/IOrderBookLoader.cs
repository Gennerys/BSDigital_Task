using MetaExchange.Models;

namespace MetaExchange.Services;

public interface IOrderBookLoader
{
    IReadOnlyList<Exchange> LoadExchanges(
        string filePath,
        int count,
        Func<string, (decimal EurBalance, decimal BtcBalance)> balanceFactory);
}
