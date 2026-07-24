using System.Globalization;
using MetaExchange.Models;
using MetaExchange.Services;

var file = "order_books_data";
var exchangeCount = 3;
var orderType = OrderType.Buy;
var amountBtc = 1m;
var eurBalance = 100_000m;
var btcBalance = 10m;

for (var i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--file" when i + 1 < args.Length:
            file = args[++i];
            break;
        case "--exchanges" when i + 1 < args.Length:
            exchangeCount = int.Parse(args[++i], CultureInfo.InvariantCulture);
            break;
        case "--type" when i + 1 < args.Length:
            orderType = Enum.Parse<OrderType>(args[++i], ignoreCase: true);
            break;
        case "--amount" when i + 1 < args.Length:
            amountBtc = decimal.Parse(args[++i], CultureInfo.InvariantCulture);
            break;
        case "--eur" when i + 1 < args.Length:
            eurBalance = decimal.Parse(args[++i], CultureInfo.InvariantCulture);
            break;
        case "--btc" when i + 1 < args.Length:
            btcBalance = decimal.Parse(args[++i], CultureInfo.InvariantCulture);
            break;
    }
}

var dataPath = Path.IsPathRooted(file)
    ? file
    : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, file));

if (!File.Exists(dataPath))
{
    Console.Error.WriteLine($"Order book file not found: {dataPath}");
    return 1;
}

Console.WriteLine($"Loading {exchangeCount} exchange(s) from: {dataPath}");
Console.WriteLine($"Order: {orderType} {Fmt(amountBtc)} BTC | Balances: EUR={Fmt(eurBalance)}, BTC={Fmt(btcBalance)}");
Console.WriteLine();

var plan = new MetaExchangeService().GetBestExecution(
    new OrderBookLoader().LoadExchanges(
        dataPath,
        exchangeCount,
        _ => (eurBalance, btcBalance)),
    orderType,
    amountBtc);

if (plan.Orders.Count == 0)
{
    Console.WriteLine("No executable orders.");
}
else
{
    foreach (var order in plan.Orders)
    {
        Console.WriteLine(
            $"  {order.OrderType} {Fmt(order.AmountBtc)} BTC @ {Fmt(order.PriceEur)} EUR " +
            $"on {order.ExchangeId} (total {Fmt(order.TotalEur)} EUR)");
    }
}

Console.WriteLine();
Console.WriteLine($"Filled: {Fmt(plan.FilledAmountBtc)} / {Fmt(plan.RequestedAmountBtc)} BTC");
Console.WriteLine($"Total EUR: {Fmt(plan.TotalEur)}");
Console.WriteLine(plan.IsFullyFilled ? "Status: FULLY FILLED" : "Status: PARTIAL FILL");

return plan.IsFullyFilled ? 0 : 2;

static string Fmt(decimal value) => value.ToString(CultureInfo.InvariantCulture);
