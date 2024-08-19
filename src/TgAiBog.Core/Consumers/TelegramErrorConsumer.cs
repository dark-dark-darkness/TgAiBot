using MassTransit;
using Microsoft.Extensions.Logging;
using TgAiBog.Contract;

namespace TgAiBog.Core.Consumers;

public sealed class TelegramErrorConsumer(ILogger<TelegramErrorConsumer> logger) : IConsumer<TelegramErrorEvent>
{
    public async Task Consume(ConsumeContext<TelegramErrorEvent> context)
    {
        logger.LogError(context.Message.Exception, "bot error");
        await Task.Delay(2000, context.CancellationToken);
    }
}