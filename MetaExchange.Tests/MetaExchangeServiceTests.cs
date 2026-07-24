using MetaExchange.Models;
using MetaExchange.Services;

namespace MetaExchange.Tests;

public sealed class MetaExchangeServiceTests
{
    private readonly MetaExchangeService _sut = new();

    [Fact]
    public void Buy_UsesCheapestAsks_CodingTaskExample()
    {
        var exchanges = new[]
        {
            ExchangeFactory.Create(
                "Ex1",
                eurBalance: 100_000m,
                btcBalance: 0m,
                asks: [(3000m, 7m), (3300m, 5m)])
        };

        var plan = _sut.GetBestExecution(exchanges, OrderType.Buy, amountBtc: 9m);

        Assert.True(plan.IsFullyFilled);
        Assert.Equal(9m, plan.FilledAmountBtc);
        Assert.Equal(27_600m, plan.TotalEur);
        Assert.Equal(2, plan.Orders.Count);
        Assert.Equal(7m, plan.Orders[0].AmountBtc);
        Assert.Equal(3000m, plan.Orders[0].PriceEur);
        Assert.Equal(2m, plan.Orders[1].AmountBtc);
        Assert.Equal(3300m, plan.Orders[1].PriceEur);
    }

    [Fact]
    public void Buy_PrefersCheaperAskAcrossExchanges()
    {
        var exchanges = new[]
        {
            ExchangeFactory.Create("Expensive", 50_000m, 0m, asks: [(3100m, 5m)]),
            ExchangeFactory.Create("Cheap", 50_000m, 0m, asks: [(3000m, 5m)])
        };

        var plan = _sut.GetBestExecution(exchanges, OrderType.Buy, amountBtc: 3m);

        Assert.True(plan.IsFullyFilled);
        Assert.Single(plan.Orders);
        Assert.Equal("Cheap", plan.Orders[0].ExchangeId);
        Assert.Equal(3000m, plan.Orders[0].PriceEur);
        Assert.Equal(3m, plan.Orders[0].AmountBtc);
        Assert.Equal(9000m, plan.TotalEur);
    }

    [Fact]
    public void Sell_PrefersHighestBidAcrossExchanges()
    {
        var exchanges = new[]
        {
            ExchangeFactory.Create("Low", 0m, 10m, bids: [(2900m, 5m)]),
            ExchangeFactory.Create("High", 0m, 10m, bids: [(3000m, 5m)])
        };

        var plan = _sut.GetBestExecution(exchanges, OrderType.Sell, amountBtc: 2m);

        Assert.True(plan.IsFullyFilled);
        Assert.Single(plan.Orders);
        Assert.Equal("High", plan.Orders[0].ExchangeId);
        Assert.Equal(3000m, plan.Orders[0].PriceEur);
        Assert.Equal(6000m, plan.TotalEur);
    }

    [Fact]
    public void Buy_SplitsAcrossExchangesWhenNeeded()
    {
        var exchanges = new[]
        {
            ExchangeFactory.Create("A", 50_000m, 0m, asks: [(3000m, 1m)]),
            ExchangeFactory.Create("B", 50_000m, 0m, asks: [(3010m, 2m)])
        };

        var plan = _sut.GetBestExecution(exchanges, OrderType.Buy, amountBtc: 3m);

        Assert.True(plan.IsFullyFilled);
        Assert.Equal(2, plan.Orders.Count);
        Assert.Equal("A", plan.Orders[0].ExchangeId);
        Assert.Equal(1m, plan.Orders[0].AmountBtc);
        Assert.Equal("B", plan.Orders[1].ExchangeId);
        Assert.Equal(2m, plan.Orders[1].AmountBtc);
        Assert.Equal(3000m + 6020m, plan.TotalEur);
    }

    [Fact]
    public void Buy_PartialFill_WhenEurBalanceInsufficient()
    {
        var exchanges = new[]
        {
            ExchangeFactory.Create("A", eurBalance: 3000m, btcBalance: 0m, asks: [(3000m, 5m)])
        };

        var plan = _sut.GetBestExecution(exchanges, OrderType.Buy, amountBtc: 5m);

        Assert.False(plan.IsFullyFilled);
        Assert.Equal(1m, plan.FilledAmountBtc);
        Assert.Equal(3000m, plan.TotalEur);
    }

    [Fact]
    public void Buy_SkipsCheapAsk_WhenThatExchangeHasNoEur()
    {
        var exchanges = new[]
        {
            ExchangeFactory.Create("NoMoney", eurBalance: 0m, btcBalance: 0m, asks: [(2900m, 10m)]),
            ExchangeFactory.Create("HasMoney", eurBalance: 50_000m, btcBalance: 0m, asks: [(3000m, 10m)])
        };

        var plan = _sut.GetBestExecution(exchanges, OrderType.Buy, amountBtc: 2m);

        Assert.True(plan.IsFullyFilled);
        Assert.Single(plan.Orders);
        Assert.Equal("HasMoney", plan.Orders[0].ExchangeId);
        Assert.Equal(3000m, plan.Orders[0].PriceEur);
    }

    [Fact]
    public void Sell_PartialFill_WhenBtcBalanceInsufficient()
    {
        var exchanges = new[]
        {
            ExchangeFactory.Create("A", eurBalance: 0m, btcBalance: 1m, bids: [(3000m, 5m)])
        };

        var plan = _sut.GetBestExecution(exchanges, OrderType.Sell, amountBtc: 5m);

        Assert.False(plan.IsFullyFilled);
        Assert.Equal(1m, plan.FilledAmountBtc);
        Assert.Equal(3000m, plan.TotalEur);
    }

    [Fact]
    public void Sell_DoesNotUseBtcFromAnotherExchange()
    {
        var exchanges = new[]
        {
            ExchangeFactory.Create("A", eurBalance: 0m, btcBalance: 0m, bids: [(3100m, 5m)]),
            ExchangeFactory.Create("B", eurBalance: 0m, btcBalance: 5m, bids: [(3000m, 5m)])
        };

        var plan = _sut.GetBestExecution(exchanges, OrderType.Sell, amountBtc: 2m);

        Assert.True(plan.IsFullyFilled);
        Assert.Single(plan.Orders);
        Assert.Equal("B", plan.Orders[0].ExchangeId);
        Assert.Equal(3000m, plan.Orders[0].PriceEur);
    }

    [Fact]
    public void Buy_EmptyOrderBook_ReturnsNoOrders()
    {
        var exchanges = new[]
        {
            ExchangeFactory.Create("Empty", eurBalance: 10_000m, btcBalance: 0m)
        };

        var plan = _sut.GetBestExecution(exchanges, OrderType.Buy, amountBtc: 1m);

        Assert.Empty(plan.Orders);
        Assert.False(plan.IsFullyFilled);
        Assert.Equal(0m, plan.FilledAmountBtc);
    }

    [Fact]
    public void Buy_IgnoresInvalidLevels()
    {
        var exchanges = new[]
        {
            ExchangeFactory.Create(
                "A",
                eurBalance: 50_000m,
                btcBalance: 0m,
                asks: [(0m, 5m), (3000m, 0m), (3100m, 2m)])
        };

        var plan = _sut.GetBestExecution(exchanges, OrderType.Buy, amountBtc: 2m);

        Assert.True(plan.IsFullyFilled);
        Assert.Single(plan.Orders);
        Assert.Equal(3100m, plan.Orders[0].PriceEur);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GetBestExecution_Throws_WhenAmountNotPositive(decimal amountBtc)
    {
        var exchanges = new[]
        {
            ExchangeFactory.Create("A", 10_000m, 10m, asks: [(3000m, 1m)])
        };

        Assert.Throws<ArgumentOutOfRangeException>(
            () => _sut.GetBestExecution(exchanges, OrderType.Buy, amountBtc));
    }

    [Fact]
    public void GetBestExecution_Throws_WhenNoExchanges()
    {
        Assert.Throws<ArgumentException>(
            () => _sut.GetBestExecution([], OrderType.Buy, amountBtc: 1m));
    }
}
