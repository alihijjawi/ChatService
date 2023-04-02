using System.ComponentModel.DataAnnotations;

namespace ChatService.Dtos;

public record SendMessageRequest(
    [Required] string SenderUsername,
    [Required] string Text);