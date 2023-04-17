using System.ComponentModel.DataAnnotations;

namespace ChatService.Dtos;

public record ConversationsList(
    [Required] ConversationDto[] Conversations,
    [Required] string NextUri);