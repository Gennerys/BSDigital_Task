using System.Text.Json.Serialization;
using BSDigital_Task.Configuration;
using BSDigital_Task.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MetaExchangeOptions>(
    builder.Configuration.GetSection(MetaExchangeOptions.SectionName));

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IMetaExchangeService, MetaExchangeService>();
builder.Services.AddSingleton<IOrderBookLoader, OrderBookLoader>();
builder.Services.AddSingleton<IExchangeMarketService, ExchangeMarketService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
