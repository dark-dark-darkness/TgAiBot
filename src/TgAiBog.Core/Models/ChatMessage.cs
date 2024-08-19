using System.ComponentModel.DataAnnotations;

namespace TgAiBog.Core.Models;

public class ChatMessage
{
    [Key]
    public Guid Id { get; set; }

    public long ChatId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string? Content { get; set; }
}