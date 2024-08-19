using LiteDB;
using LiteDB.Async;
using MassTransit;
using Microsoft.SemanticKernel;
using TgAiBog.Core.Consumers;
using TgAiBog.Core.Extensions;
using TgAiBog.Core.Services;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddKernel()
    .AddAzureOpenAIChatCompletion(
        builder.Configuration["OpenAI:DeploymentName"]!,
        builder.Configuration["OpenAI:Endpoint"]!,
        builder.Configuration["OpenAI:ApiKey"]!);
builder.Services.AddTelegramBot("BotToken");
builder.Services.AddSingleton<IChatHistoryService, ChatHistoryService>();
builder.Services.AddSingleton<ILiteDatabaseAsync>(_ =>
    new LiteDatabaseAsync(new ConnectionString(builder.Configuration.GetConnectionString("LiteDb"))
    {
        Connection = ConnectionType.Shared
    }));
builder.Services.AddMassTransit(c =>
{
    c.SetKebabCaseEndpointNameFormatter();
    c.AddConsumer<TelegramErrorConsumer>();
    c.AddConsumer<TelegramUpdateConsumer>();
    c.AddConsumer<TelegramMessageConsumer>();
    c.AddConsumer<ChatMessageConsumer>(x => { x.UseRateLimit(6, TimeSpan.FromMinutes(1)); });
    c.UsingInMemory((ctx, cfg) => cfg.ConfigureEndpoints(ctx));
});

await builder.Build().RunAsync();