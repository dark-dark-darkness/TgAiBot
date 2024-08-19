using System.Collections.Concurrent;
using LiteDB.Async;
using MassTransit;
using MassTransit.Internals;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using TgAiBog.Core.Models;

namespace TgAiBog.Core.Services;

public class ChatHistoryService(ILiteDatabaseAsync db) : IChatHistoryService
{
    private readonly ILiteCollectionAsync<ChatMessage> _collection = db.GetCollection<ChatMessage>();

    public async Task<ChatHistory> GetChatHistoryAsync(long chatId)
    {
        var collection = await _collection.FindAsync(x => x.ChatId == chatId);
        return new ChatHistory(collection.Select(x => new ChatMessageContent(new AuthorRole(x.Role), x.Content)));
    }

    public async Task AddMessageAsync(long chatId, string username, string message, AuthorRole role)
    {
        await _collection.InsertAsync(new ChatMessage
        {
            Id = NewId.NextSequentialGuid(),
            ChatId = chatId,
            Content = !string.IsNullOrWhiteSpace(username) ? $"{username}| {message}" : message,
            Role = role.Label,
        });
    }
}