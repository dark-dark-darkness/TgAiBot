using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgAiBog.Contract;
using TgAiBog.Core.Services;

namespace TgAiBog.Core.Consumers;

public sealed class TelegramMessageConsumer(
    ILogger<TelegramMessageEvent> logger,
    TelegramBotClient bot,
    IChatHistoryService historyService
) : IConsumer<TelegramMessageEvent>
{
    public async Task Consume(ConsumeContext<TelegramMessageEvent> context)
    {
        await OnMessage(context.Message.Message, context.Message.UpdateType, context.CancellationToken);
    }

    async Task OnMessage(Message msg, UpdateType type, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(msg.Text))
        {
            var username = msg.From.FirstName + msg.From.LastName;
            var text = msg.Text.StartsWith("/chat") ? msg.Text.Substring("/chat".Length).Trim() : msg.Text.Trim();
            logger.LogInformation("Received a message of type {MessageType}", msg.Type);
            await historyService.AddMessageAsync(msg.Chat.Id, username, text,
                AuthorRole.User);
        }
    }
}