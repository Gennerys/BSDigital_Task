using System.ComponentModel.DataAnnotations;
using MetaExchange.Models;

namespace MetaExchange.Contracts;

public sealed class BestExecutionRequest
{
    [Required]
    public OrderType OrderType { get; init; }

    [Range(typeof(decimal), "0.00000001", "1000000")]
    public decimal AmountBtc { get; init; }
}
