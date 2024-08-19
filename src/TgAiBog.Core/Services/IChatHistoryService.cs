using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace TgAiBog.Core.Services;

public interface IChatHistoryService
{
    public Task<ChatHistory> GetChatHistoryAsync(long chatId);

    public Task AddMessageAsync(long chatId, string username, string message, AuthorRole role);
}