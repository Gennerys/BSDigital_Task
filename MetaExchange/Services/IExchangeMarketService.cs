using MetaExchange.Models;

namespace MetaExchange.Services;

public interface IExchangeMarketService
{
    IReadOnlyList<Exchange> GetExchanges();
}
