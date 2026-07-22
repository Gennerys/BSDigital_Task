using BSDigital_Task.Models;

namespace BSDigital_Task.Services;

public interface IExchangeMarketService
{
    IReadOnlyList<Exchange> GetExchanges();
}
