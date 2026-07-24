using MetaExchange.Models;

namespace MetaExchange.Services;

public interface IMetaExchangeService
{
    ExecutionPlan GetBestExecution(
        IReadOnlyList<Exchange> exchanges,
        OrderType orderType,
        decimal amountBtc);
}
