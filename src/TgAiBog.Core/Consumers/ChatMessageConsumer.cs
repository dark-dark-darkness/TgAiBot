using MassTransit;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Telegram.Bot;
using TgAiBog.Contract;
using TgAiBog.Core.Services;

namespace TgAiBog.Core.Consumers;

public sealed class ChatMessageConsumer(
    IChatHistoryService historyService,
    IChatCompletionService completionService,
    Kernel kernel,
    TelegramBotClient bot
) : IConsumer<TelegramMessageEvent>
{
    private readonly PromptExecutionSettings _settings = new();

    public async Task Consume(ConsumeContext<TelegramMessageEvent> context)
    {
        var (_, msg, _) = context.Message;

        if (msg is not { Text: { } })
        {
            return;
        }

        if (msg.Text.StartsWith("/chat"))
        {
            var chatId = msg.Chat.Id;
            var text = msg.Text["/chat".Length..].Trim();
            var username = msg.From.FirstName + msg.From.LastName;

            if (!string.IsNullOrWhiteSpace(text))
            {
                var history = await historyService.GetChatHistoryAsync(chatId);
                history.AddSystemMessage("|前面的是说话人的名字，|后面的是说话的内容");
                history.AddMessage(AuthorRole.User, $"{username}| {text}");
                var result = await completionService
                    .GetChatMessageContentAsync(history, _settings, kernel);

                if (result.Content is not null)
                {
                    await historyService.AddMessageAsync(chatId, "", result.Content, AuthorRole.Assistant);
                    await bot.SendTextMessageAsync(msg.Chat.Id, result.Content);
                }
            }
        }
    }
}