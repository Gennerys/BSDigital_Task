using System.ComponentModel.DataAnnotations;
using BSDigital_Task.Models;

namespace BSDigital_Task.Contracts;

public sealed class BestExecutionRequest
{
    [Required]
    public OrderType OrderType { get; init; }

    [Range(typeof(decimal), "0.00000001", "1000000")]
    public decimal AmountBtc { get; init; }
}
