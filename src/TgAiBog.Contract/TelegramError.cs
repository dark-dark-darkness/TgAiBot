using Telegram.Bot.Polling;

namespace TgAiBog.Contract;

public sealed record TelegramErrorEvent(
    Guid ErrorId,
    Exception Exception,
    HandleErrorSource Source
);