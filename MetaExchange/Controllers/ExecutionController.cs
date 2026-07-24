using MetaExchange.Contracts;
using MetaExchange.Services;
using Microsoft.AspNetCore.Mvc;

namespace MetaExchange.Controllers;

[ApiController]
[Route("api/execution")]
public sealed class ExecutionController : ControllerBase
{
    private readonly IMetaExchangeService _metaExchange;
    private readonly IExchangeMarketService _market;

    public ExecutionController(
        IMetaExchangeService metaExchange,
        IExchangeMarketService market)
    {
        _metaExchange = metaExchange;
        _market = market;
    }

    [HttpPost("best")]
    [ProducesResponseType(typeof(BestExecutionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<BestExecutionResponse> GetBestExecution([FromBody] BestExecutionRequest request)
    {
        if (request.AmountBtc <= 0)
            return BadRequest("Amount must be positive.");

        var exchanges = _market.GetExchanges();
        var plan = _metaExchange.GetBestExecution(exchanges, request.OrderType, request.AmountBtc);
        return Ok(BestExecutionResponse.From(plan));
    }
}
