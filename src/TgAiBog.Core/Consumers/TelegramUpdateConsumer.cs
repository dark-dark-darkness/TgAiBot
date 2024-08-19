using MassTransit;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgAiBog.Contract;

namespace TgAiBog.Core.Consumers;

public sealed class TelegramUpdateConsumer(
    ILogger<TelegramUpdateConsumer> logger,
    TelegramBotClient bot
) : IConsumer<TelegramUpdateEvent>
{
    public async Task Consume(ConsumeContext<TelegramUpdateEvent> context)
    {
        await OnUpdate(context.Message.Update);
    }

    async Task OnUpdate(Update update)
    {
        switch (update)
        {
            case { CallbackQuery: { } callbackQuery }:
                await OnCallbackQuery(callbackQuery);
                break;
            case { PollAnswer: { } pollAnswer }:
                await OnPollAnswer(pollAnswer);
                break;
            default:
                logger.LogInformation("Received unhandled update {UpdateType}", update.Type);
                break;
        }
    }


    async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        await bot.AnswerCallbackQueryAsync(callbackQuery.Id, $"You selected {callbackQuery.Data}");
        await bot.SendTextMessageAsync(callbackQuery.Message!.Chat,
            $"Received callback from inline button {callbackQuery.Data}");
    }

    async Task OnPollAnswer(PollAnswer pollAnswer)
    {
        if (pollAnswer.User != null)
            await bot.SendTextMessageAsync(pollAnswer.User.Id,
                $"You voted for option(s) id [{string.Join(',', pollAnswer.OptionIds)}]");
    }
}