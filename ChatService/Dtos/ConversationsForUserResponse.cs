using System.ComponentModel.DataAnnotations;

namespace ChatService.Dtos;

public record ConversationsForUserResponse(
    [Required] ConversationDto[] Conversations,
    [Required] string NextUri);