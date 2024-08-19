using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TgAiBog.Contract;

public sealed record TelegramMessageEvent(
    Guid MessageId,
    Message Message,
    UpdateType UpdateType
);