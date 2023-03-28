using System.ComponentModel.DataAnnotations;

namespace ChatService.Dtos;

public record SendMessageRequest(
    [Required] int MessageId,
    [Required] string SenderUsername,
    [Required] string Text);