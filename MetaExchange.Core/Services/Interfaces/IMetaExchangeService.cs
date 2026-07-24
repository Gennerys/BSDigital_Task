using MetaExchange.Models;

namespace MetaExchange.Services;

/// <summary>
/// Computes the best-execution plan for buying or selling BTC across multiple exchanges.
/// </summary>
public interface IMetaExchangeService
{
    /// <summary>
    /// Builds an execution plan that fills the requested BTC amount at the best available price,
    /// respecting each exchange's EUR and BTC balances.
    /// </summary>
    /// <param name="exchanges">Exchanges with order books and available balances.</param>
    /// <param name="orderType">Whether to buy or sell BTC.</param>
    /// <param name="amountBtc">Requested BTC amount; must be positive.</param>
    /// <returns>
    /// An execution plan containing one or more orders. The filled amount may be less than
    /// requested when liquidity or balances are insufficient.
    /// </returns>
    ExecutionPlan GetBestExecution(
        IReadOnlyList<Exchange> exchanges,
        OrderType orderType,
        decimal amountBtc);
}
