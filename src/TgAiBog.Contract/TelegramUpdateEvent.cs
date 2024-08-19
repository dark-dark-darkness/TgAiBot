using Telegram.Bot.Types;

namespace TgAiBog.Contract;

public sealed record TelegramUpdateEvent(
    Guid UpdateId,
    Update Update
);