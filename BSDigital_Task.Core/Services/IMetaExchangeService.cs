using BSDigital_Task.Models;

namespace BSDigital_Task.Services;

public interface IMetaExchangeService
{
    ExecutionPlan GetBestExecution(
        IReadOnlyList<Exchange> exchanges,
        OrderType orderType,
        decimal amountBtc);
}
